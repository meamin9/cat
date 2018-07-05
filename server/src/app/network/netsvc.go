package network

import (
	"cellnet"
	"cellnet/peer"
	"app"
	"cellnet/proc"
)

type Session cellnet.Session

type NetSvc struct {
	tcp cellnet.Peer
	addr string

	handlerQue  chan func()
	HandlerById map[int]func(Session, interface{})
}

func (self *NetSvc) Init() {
	self.handlerQue = make(chan func(), 128)
	self.HandlerById = make(map[int]func(Session, interface{}))
	self.tcp = peer.NewGenericPeer("tcp.Acceptor", "server-cat", self.addr, nil)
	proc.BindProcessorHandler(self.tcp, "tcp.ltv", func(event cellnet.Event) {
		ses := event.Session().(Session)
		msg := event.Message()
		if handler, ok := self.HandlerById[cellnet.MessageToID(msg)]; ok {
			self.handlerQue <- func() {
				handler(ses, msg)
			}
		}
	})
}

func (self *NetSvc) Start() {
	self.tcp.Start()
}

func (self *NetSvc) Stop() {
	self.tcp.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
	// n := self.host.(interface{ Count() int }).Count()
	//self.exitSync.Add(n)
}

func (self *NetSvc) RegProto(msgId int, cb func(Session, interface{})) {
	if _, ok := self.HandlerById[msgId]; ok {
		panic("proto is register repeated")
	}
	self.HandlerById[msgId] = cb
}

func (self *NetSvc) UnregProto(key int) {
	delete(self.HandlerById, key)
}


func (self *NetSvc) Chan() chan func() {
	return self.handlerQue
}

func (self *NetSvc) Call(handler func()) {
	handler()
}

func (self *NetSvc) Pull() {
	for {
		select {
		case handler := <- self.handlerQue:
			handler()
		default:
			break
		}
	}
}

var Instance * NetSvc
func init() {
	Instance = &NetSvc{}
	app.Master.RegService(Instance, false)
}
