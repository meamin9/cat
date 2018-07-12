package network

import (
	"cellnet"
	"cellnet/peer"
	_ "cellnet/peer/tcp"
	"cellnet/proc"
	_ "cellnet/proc/tcp"
	"proto"
	"app/appinfo"
)

type Session cellnet.Session

type ISender interface {
	Send(msg interface{})
}

type NetMgr struct {
	tcp cellnet.Peer
	// POST队列
	handlerQue chan func()
	// 协议注册表，不加锁，网络开始前全部注册好
	handlerByIds map[int]func(Session, interface{})
}

func (self *NetMgr) Init() {
	self.handlerQue = make(chan func(), 128)
	self.handlerByIds = make(map[int]func(Session, interface{}))
	self.tcp = peer.NewGenericPeer("tcp.Acceptor", "server-cat", appinfo.ServerAddr, nil)
	proc.BindProcessorHandler(self.tcp, "tcp.ltv", func(event cellnet.Event) {
		ses := event.Session().(Session)
		msg := event.Message()
		msgId := cellnet.MessageToID(msg)
		switch msgId {
		case CodeSessionAccepted:
			recvSessionAccepted(ses, msg)
		case proto.CodeCSEntryToken:
			recvEntryToken(ses, msg)
		default:
			token, ok := ses.(peer.ICoreContextSet).RawGetContext("token")
			if !ok || !token.(*entryToken).Ok { // 非法连接
				ses.Close()
				return
			}
			if handler, ok := self.handlerByIds[msgId]; ok {
				self.handlerQue <- func() {
					handler(ses, msg)
				}
			}
		}
	})
	regSystemMsg()
}

func (self *NetMgr) Start() {
	self.tcp.Start()
}

func (self *NetMgr) Stop() {
	self.tcp.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
	// n := self.host.(interface{ Count() int }).Count()
	//self.exitSync.Add(n)
}

func (self *NetMgr) RegProto(msgId int, cb func(Session, interface{})) {
	if _, ok := self.handlerByIds[msgId]; ok {
		panic("proto is register repeated")
	}
	self.handlerByIds[msgId] = cb
}

func (self *NetMgr) UnregProto(key int) {
	delete(self.handlerByIds, key)
}

func (self *NetMgr) Chan() chan func() {
	return self.handlerQue
}

func (self *NetMgr) Call(handler func()) {
	handler()
}

// 处理完队列里的消息
func (self *NetMgr) Flush() {
	for {
		select {
		case handler := <-self.handlerQue:
			handler()
		default:
			break
		}
	}
}

var Instance *NetMgr

func New() *NetMgr {
	Instance = &NetMgr{}
	return Instance
}
