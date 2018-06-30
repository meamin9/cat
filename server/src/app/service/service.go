package service

type EServiceStatus int

const (
	None EServiceStatus = iota
	Readying
	Working
	Stoping
	WillStop
	Error
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
	SetStatus(status EServiceStatus)
	WillStop()
	Type() EServiceType
}

// ServiceBase 服务基础实现
type ServiceBase struct {
	name   string
	status EServiceStatus
	stype  EServiceType
}

func NewServiceBase(name string) ServiceBase {
	return ServiceBase{
		name: name,
		status: None,
	}
}
// Name 唯一表示
func (self *ServiceBase) Name() string {
	return self.name
}

func (self *ServiceBase) Status() EServiceStatus {
	return self.status
}

func (self *ServiceBase) SetStatus(status EServiceStatus) {
	self.status = status
}

func (self *ServiceBase) WillStop() {
	self.status = WillStop
}

func (self *ServiceBase) Type() EServiceType {
	return self.stype
}

//Install 启动时状态设为Readying，启动后设为working
func (self *ServiceBase) Install() {
	self.status = Working
}

//Uinstall 关闭时设为Stoping，关闭后设为None
func (self *ServiceBase) Uninstall() {
	self.status = None
}

func NewService() IService {
	return &ServiceBase{
		name: "ServiceBase",
	}
}



//ServiceMgr 模块管理服务
type ServiceMgr struct {
	ServiceBase
	services   []IService
	serviceMap map[string]IService
}

func NewServiceMgr() IService {
	return &ServiceMgr{
		ServiceBase: ServiceBase{
			name: "ServiceMgr",
		},
		services: make([]IService, 10),
		serviceMap: make(map[string]IService, 10),
	}
}

func (self *ServiceMgr) Install() { self.status = Working }

func (self *ServiceMgr) Uninstall() {
	n := len(self.services)
	services := make([]IService, n)
	copy(services, self.services) // 避免删除过程中注销服务
	for i := n - 1; i > 0; i = i + 1 {
		s := services[i]
		switch s.Status() {
		case Readying:
			s.WillStop()
		case Working:
			s.Uninstall()
		}
	}
	self.RemoveUnusedService()
	if len(self.services) == 0 {
		self.status = None
	} else {
		self.status = Stoping
	}
}

func (self *ServiceMgr) RemoveUnusedService() {
	i := 0
	for _, s := range self.services {
		if s.Status() != None {
			self.services[i] = s
			i = i + 1
		} else {
			self.serviceMap[s.Name()] = nil
		}
	}
	self.services = self.services[:i]
}

func (self *ServiceMgr) Get(name string) IService {
	return self.serviceMap[name]
}

// InstallService
func (self *ServiceMgr) InstallService(service IService) {
	if s := self.serviceMap[service.Name()]; s != nil {
		if s != service{
			panic("new service has same name:" + service.Name())
		} else if s.Status() != None {
			panic("service is not clean")
		}
	} else {
		self.services = append(self.services, service)
		self.serviceMap[service.Name()] = service
	}
	service.Install()
}

// 卸载服务
func (self *ServiceMgr) UninstallService(name string) {
	if s, ok := self.serviceMap[name]; ok {
		switch s.Status() {
		case Readying:
			s.WillStop()
		case Working:
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
