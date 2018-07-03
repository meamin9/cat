package app

// 所有服务在import这里导入（无序），自身的init里注册Cfg和Svc
import (
	"app/network"
	"app/db"
	"app/apptime"
)

type ICfg interface {
	LoadCfg()
}

type ISender interface {
	Send(msg interface{})
}

type App struct {
	*ServiceMgr
	*AppCfg
	exitC chan bool
}

var Master *App

func newapp() *App {
	return &App{
		ServiceMgr: NewServiceMgr(),
		exitC: make(chan bool,),
	}
}

// 初始化配置（不要lazy初始化）
// 初始化db，net
// 各模块自身初始化，可以依赖配置，网络，加载db, 此时网络和db的异步go程还没开
// 各模块开始运行，此时所有数据都已正常初始化，start中可以按照正常数据进行逻辑
// 启动网络和db线程，进入游戏主循环
// 关闭网络
// 关闭其他模块
// 关闭db
func (self *App) Start() {
	self.VisitService(func(s IService) {
		if cfg, ok := s.(ICfg); ok {
			cfg.LoadCfg()
		}
	})
	network.Instance.Init()
	db.Instance.Init()
	self.VisitService(func(s IService) {
		s.Init()
	})
	self.VisitService(func(s IService) {
		s.Start()
	})
	db.Instance.Start()
	network.Instance.Start()
	for {
		select {
		case proc := <- network.Instance.C():
			proc()
		case proc := <- db.Instance.C():
			proc()
		case t := <- apptime.Instance.C():
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
	newapp().Start()
}
