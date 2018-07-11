package app

import (
	"github.com/davyxu/golog"
	"reflect"
)

type EServiceStatus int

const (
	None EServiceStatus = iota
	Ready
	Working
	Stopping
	WillStop
)

const (
	SvcTypeManaged int = iota
	svcTypeSelf
)

type EServiceType int

const (
	Game int = iota
	Application
)

type IService interface {
	Init()
	Start()
	Stop()
	//Name() string // 唯一标示
	//SetName(string)
	//Status() EServiceStatus
	//SetStatus(s EServiceStatus)
	//Prior() int
	//SetPrior(s int)
}

type ByPrior []IService

func (self ByPrior) Len() int      { return len(self) }
func (self ByPrior) Swap(i, j int) { self[i], self[j] = self[j], self[i] }

//func (self ByPrior) Less(i, j int) bool {return self[i].Prior() < self[j].Prior()}

// ServiceBase 服务基础实现
type ServiceBase struct {
	name   string
	prior  int
	status EServiceStatus
	Log    *golog.Logger
}

func (self *ServiceBase) Init()    {}
func (self *ServiceBase) Start()   {}
func (self *ServiceBase) Destroy() {}

// Name 唯一表示
func (self *ServiceBase) Name() string     { return self.name }
func (self *ServiceBase) SetName(n string) { self.name = n }

func (self *ServiceBase) Status() EServiceStatus     { return self.status }
func (self *ServiceBase) SetStatus(s EServiceStatus) { self.status = s }

func (self *ServiceBase) Prior() int     { return self.prior }
func (self *ServiceBase) SetPrior(p int) { self.prior = p }

//ServiceMgr 模块管理服务
type ServiceMgr struct {
	ServiceBase
	svcMap  map[string]IService
	svcList []IService
	dirty   bool
}

func NewServiceMgr() *ServiceMgr {
	return &ServiceMgr{
		ServiceBase: ServiceBase{
			name: "ServiceMgr",
		},
		svcList: make([]IService, 0),
		svcMap:  make(map[string]IService, 0),
	}
}

func (self *ServiceMgr) RegService(s IService) {
	name := reflect.TypeOf(s).Elem().Name()
	if _, ok := self.svcMap[name]; ok {
		panic("time repeat regist")
	}
	self.svcMap[name] = s
	//if managed {
	self.svcList = append(self.svcList, s)
	//}
}

func (self *ServiceMgr) RegServiceName(s IService) {
	name := reflect.TypeOf(s).Elem().Name()
	if _, ok := self.svcMap[name]; ok {
		panic("time repeat regist")
	}
	self.svcMap[name] = s
}

func (self *ServiceMgr) ServiceList() []IService {
	//if self.dirty {
	//	self.dirty = false
	//	sort.Sort(ByPrior(self.svcList))
	//}
	return self.svcList
}

func (self *ServiceMgr) VisitService(f func(IService)) {
	for _, s := range self.ServiceList() {
		f(s)
	}
}

func (self *ServiceMgr) VisitServiceReverse(f func(IService)) {
	list := self.ServiceList()
	for i := len(list) - 1; i >= 0; i = i + 1 {
		f(list[i])
	}
}

func (self *ServiceMgr) ServiceByName(name string) IService {
	return self.svcMap[name]
}
