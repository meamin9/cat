package app

// 所有服务在import这里导入（无序），自身的init里注册Cfg和Svc
import (
	_ "app/network"
	_ "app/db"
)

type ICfg interface {
	LoadCfg()
}

type App struct {
	*ServiceMgr
	cfgs []ICfg
	exit chan bool
}

var Master *App

func newapp() *App {
	return &App{
		cfgs: make([]ICfg, 0),
		ServiceMgr: NewServiceMgr(),
		exit: make(chan bool),
	}
}

// 初始化配置（不要lazy初始化）
// 启动db，net
// 各模块自身初始化，可以依赖配置，网络，加载db
// 各模块运行，此时所有数据都已正常初始化
// 游戏主循环
// 关闭网络
// 关闭其他模块
// 关闭db
func (self *App) Start() {
	for _, cfg := range self.cfgs {
		cfg.LoadCfg()
	}
	self.VisitService(func(s IService) {
		s.Init()
	})
	self.VisitService(func(s IService) {
		s.Start()
	})
	self.VisitServiceReverse(func(s IService) {
		s.Stop()
	})
}

func (self *App) Stop() {

}

func main() {
	newapp().Start()
}
