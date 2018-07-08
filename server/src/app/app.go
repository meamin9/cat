package app

// 所有服务在import这里导入（无序），自身的init里注册Cfg和Svc
import (
	"app/network"
	"app/db"
	"app/apptime"
	"app/account"
	"app/role"
	"sync"
)

type ICfg interface {
	LoadCfg()
}

type App struct {
	*ServiceMgr
	*AppCfg
	exitC chan bool
	// path
}

var Instance *App

func newApp() *App {
	Instance = &App{
		ServiceMgr: NewServiceMgr(),
		exitC: make(chan bool,),
	}
	return Instance
}


// 改成手动初始化，不用包内的init初始化了（顺序不容易确定）
func (self *App) initPackage() {
	network.New()
	db.New()
	// 统一管理的module
	self.svcList = []IService{
		account.New(),
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
func (self *App) Start() {
	self.LoadCfg()
	self.initPackage()

	// loadcfg和init阶段不同包不会交互，逻辑上线程安全
	loading := sync.WaitGroup{}
	n := len(self.svcList)
	loading.Add(n)
	for _, s := range self.svcList {
		go func() {
			if cfg, ok := s.(ICfg); ok {
				cfg.LoadCfg()
			}
			loading.Done()
		}()
	}
	loading.Wait()

	// package init
	network.Instance.Init()
	db.Instance.Init()
	loading.Add(n)
	for _, s := range self.svcList {
		go func() {
			s.Init()
			loading.Done()
		}()
	}
	loading.Wait()

	// package start
	for _, s := range self.svcList {
		s.Start()
	}
	db.Instance.Start()
	network.Instance.Start()

	// main loop
	for {
		select {
		case proc := <- network.Instance.Chan():
			proc()
		case proc := <- db.Instance.Chan():
			proc()
		case t := <- apptime.Instance.Chan():
			apptime.Instance.Tick(t)
		case <- self.exitC:
			break
		}
	}

	network.Instance.Stop()
	self.VisitServiceReverse(func(s IService) {
		s.Stop()
	})
	db.Instance.Stop()
}

func (self *App) Stop() {
	self.exitC <- true
}

func main() {
	newApp().Start()
}
