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

func NewNetSvc() *NetSvc {
	return &NetSvc{
		ServiceBase: app.NewServiceBase("NewNetWork"),
		addr: "127.0.0.1:7200",
	}
}

func (self *NetSvc) Install() {
	self.ServiceBase.Install()
	self.queue = cellnet.NewEventQueue()
	self.host = peer.NewGenericPeer("tcp.Acceptor", "server", self.addr, self.queue)
	proc.BindProcessorHandler(self.host, "tcp.ltv", func(ev cellnet.Event) {
		self.ProcProto(ev)
	})
}

func (self *NetSvc) Uninstall() {
	self.SetStatus(app.Stopping)
	self.host.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
	// n := self.host.(interface{ Count() int }).Count()
	//self.exitSync.Add(n)
	self.SetStatus(app.None)
}

func (self *NetSvc) Run() {
	self.host.Start()
}

