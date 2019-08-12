package app

import (
	_ "app/appinfo"
	"app/apptime"
	"app/db"
	"app/db/collection"
	"app/mosaic"
	"app/role"
	"app/user"
	"app/util"
	"sync"
)


var DEBUG = true

type App struct {
	packList []interface{}
	exitC chan bool
}

var log *util.Logger
func newApp() *App {
	log = util.NewLog("app")
	return &App{
		exitC:      make(chan bool),
	}
}

// 改成手动初始化，不用包内的init初始化了（顺序不容易确定）
func (app *App) initPackage() {
	user.New()
	db.New()
	// 统一管理的module
	app.packList = []interface{}{
		collection.New(),
		apptime.New(),
		mosaic.New(),
		user.New(),
		role.New(),
	}
}

// 初始化配置（不要lazy初始化）
// 初始化db，net
// 各模块自身初始化，可以依赖配置，网络，加载db, 此时网络和db的异步go程还没开
// 各模块开始运行，此时所有数据都已正常初始化，start中可以按照正常数据进行逻辑
// 启动网络和db线程，进入游戏主循环
// 关闭网络
// flush db的回调队列，保证各模块数据都是最新的，关闭其他模块
// 关闭db
func (app *App) Start() {
	log.Infoln("App Enter")
	app.initPackage()
	// load和init阶段不同包不会交互，逻辑上线程安全
	loading := sync.WaitGroup{}
	n := len(app.packList)
	loading.Add(n)
	for _, pack := range app.packList {
		pack := pack // 这里需要新建一个临时变量
		go func() {
			if cfg, ok := pack.(interface{PrevInit()}); ok {
				cfg.PrevInit()
			}
			loading.Done()
		}()
	}
	loading.Wait()

	// package init
	user.Instance.Init()
	db.Instance.Init()
	loading.Add(n)
	for _, s := range app.packList {
		pack := s
		go func() {
			if cfg, ok := pack.(interface{Init()}); ok {
				cfg.Init()
			}
			loading.Done()
		}()
	}
	loading.Wait()

	// package start
	for _, s := range app.packList {
		if cfg, ok := s.(interface{PostInit()}); ok {
			cfg.PostInit()
		}
	}
	db.Instance.Start()
	user.Instance.Start()

	// main loop
Loop:
	for {
		select {
		case proc := <-user.Instance.Chan():
			proc()
		case proc := <-db.Instance.Chan():
			proc()
		case t := <-apptime.Instance.Chan():
			apptime.Instance.Tick(t)
		case <-app.exitC:
			break Loop
		}
	}
	// Stop
	user.Instance.Stop()
	for _, s := range app.packList {
		if cfg, ok := s.(interface{Stop()}); ok {
			cfg.Stop()
		}
	}
	db.Instance.Stop()
	log.Infoln("App Exited")
}

func (app *App) Stop() {
	app.exitC <- true
}

func main() {
	newApp().Start()
}
