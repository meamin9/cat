package network

import (
	"cellnet"
	"cellnet/peer"
	"app"
	"cellnet/proc"
	"sync"
)

type Session cellnet.Session

type NetSvc struct {
	app.ServiceBase
	host cellnet.Peer
	addr string
	queue cellnet.EventQueue
	exitSync sync.WaitGroup

	dispatcher
}

func (self *NetSvc) Init() {
	self.queue = cellnet.NewEventQueue()
	self.host = peer.NewGenericPeer("tcp.Acceptor", "server-cat", self.addr, self.queue)
	proc.BindProcessorHandler(self.host, "tcp.ltv", func(ev cellnet.Event) {
		self.ProcProto(ev)
	})
}

func (self *NetSvc) Start() {
	self.host.Start()
}

func (self *NetSvc) Stop() {
	self.host.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
	// n := self.host.(interface{ Count() int }).Count()
	//self.exitSync.Add(n)
}

func (self *NetSvc) Pull() {

}

var svc * NetSvc
func init() {
	svc = &NetSvc{}
	app.Master.RegService(svc, "network", app.PriorBase)
}
