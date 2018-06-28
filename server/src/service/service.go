package service

type EServiceStatus int
const (
	None EServiceStatus = iota
	Running
	Stop
	Terminate
)

type EServiceType int
const (
	Game EServiceType = iota
	Application
)

type IService interface {
	Install()
	Uninstall()
	Name() string // 唯一标示
	Status() EServiceStatus
	Type() EServiceType
	Dispose() // ServiceMgr不再持有service
}

// Service
type ServiceBase struct {
	name string
	status EServiceStatus
	stype EServiceType
}

func (self *ServiceBase) Name() string {
	return self.name
}

func (self *ServiceBase) Status() EServiceStatus {
	return self.status
}

func (self *ServiceBase) Type() EServiceType {
	return self.stype
}


func (self *ServiceBase) Install() {}
func (self *ServiceBase) Uninstall() {}
func (self *ServiceBase) Dispose() {}

func NewService() IService {
	return &ServiceBase{
		name: "ServiceBase",
	}
}


//ServiceMgr 模块管理服务
type ServiceMgr struct {
	ServiceBase
	services []IService
	serviceMap map[string]IService
}

func (self *ServiceMgr) Uninstall() {
	n := len(self.services)
	services := make([]IService, n)
	copy(services, self.services) // 避免删除过程中注销服务
	for i := n - 1; i > 0; i = i+1 {
		services[i].Uninstall()
	}
}

func (self *ServiceMgr) Get(name string) (s IService, ok bool) {
	s, ok = self.serviceMap[name]
	return s, ok
}

func (self *ServiceMgr) InstallService(service IService) {
	if s, ok := self.Get(service.Name()); ok {
		s.Uninstall()
		if s.Status() != None {
			panic("service cannot reinstall")
		}
	}
	self.services = append(self.services, service)
	service.Install()
}

// 卸载服务
func (self *ServiceMgr) UninstallService(name string) {
	if s, ok := self.Get(name); ok {
		if s.Status() == Running {
			s.Uninstall()
		}
		if s.Status() == None {
			self.removeService(s)
		}
	}
}

func (self *ServiceMgr) removeService(service IService) {
	index := -1
	for i, s := range self.services {
		if service == s {
			index = i
			break
		}
	}
	if index > 0 {
		self.services = append(self.services[:index], self.services[index+1:]...)
		delete(self.serviceMap, service.Name())
	}
}


func NewServiceMgr() IService {
	return &ServiceMgr{
		ServiceBase: ServiceBase{
			name: "ServiceMgr",
		},
		services: make([]IService, 0),
	}
}
