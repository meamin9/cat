package app

import (
	"app/db"
	"app/fw"
	_ "app/fw/appinfo"
	"app/user"
	"sync"
)


var DEBUG = true

type App struct {
	packList []interface{}
	signal   chan bool
}

var log *fw.Logger
func newApp() *App {
	log = fw.NewLog("app")
	return &App{
		signal: make(chan bool),
	}
}

func (app *App) initPackage() {
	//
	app.packList = []interface{}{
		//mosaic.New(),
		//user.New(),
		//role.New(),
	}
}

func (app *App) Start() {
	log.Infoln("App Enter")
	app.initPackage()

	// 初始化
	loadGroup := sync.WaitGroup{}
	loadSignal := make(chan bool)
	// 1. 启动db
	db.Manager.StartLoop()
	// db 事件
	go func() {
		for {
			select {
			case proc := <-db.Manager.EventChan():
				proc()
			case <- loadSignal:
				return
			}
		}
	}()

	// 2. 加载各个模块，读库等操作
	step := func() {
		loadGroup.Done()
	}
	for _, s := range app.packList {
		if cfg, ok := s.(interface{Initialize(func())}); ok {
			loadGroup.Add(1)
			go cfg.Initialize(step)
		}
	}

	loadGroup.Wait()
	loadSignal <- true //退出db事件线程

	// 启动网络
	user.Manager.Start()
	fw.Manager.Start()
	// main loop
Loop:
	for {
		select {
		case proc := <-user.Manager.EventChan():
			proc()
		case proc := <-db.Manager.EventChan():
			proc()
		case t := <-fw.Manager.TickChan():
			fw.Manager.Tick(t)
		case <-app.signal:
			break Loop
		}
	}
	// Cancel 网络
	user.Manager.Stop()
	for _, s := range app.packList {
		if cfg, ok := s.(interface{Save()}); ok {
			cfg.Save()
		}
	}
	db.Manager.StopLoop()
	db.Manager.Wait()
	log.Infoln("App Exited")
}

func (app *App) Stop() {
	app.signal <- true
}

func main() {
	newApp().Start()
}
