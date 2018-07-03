package app

// 所有服务在import这里导入（无序），自身的init里注册Cfg和Svc
import (
	"app/network"
	"app/db"
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
	exit chan bool
}

var Master *App

func newapp() *App {
	return &App{
		ServiceMgr: NewServiceMgr(),
		exit: make(chan bool),
	}
}

// 初始化配置（不要lazy初始化）
// 初始化db，net
// 各模块自身初始化，可以依赖配置，网络，加载db
// 各模块运行，此时所有数据都已正常初始化
// 游戏主循环
// 关闭网络
// 关闭其他模块
// 关闭db
func (self *App) Start() {
	self.VisitService(func(s IService) {
		if cfg, ok := s.(ICfg); ok {
			cfg.LoadCfg()
		}
	})
	self.VisitService(func(s IService) {
		s.Init()
	})
	self.VisitService(func(s IService) {
		s.Start()
	})
	for {
		network.Svc.Pull()
		db.Svc.Pull()
	}
	self.VisitServiceReverse(func(s IService) {
		s.Stop()
	})
}

func (self *App) Stop() {

}

func main() {
	newapp().Start()
}
