package user

import (
	"app/appinfo"
	"github.com/davyxu/cellnet"
	"github.com/davyxu/cellnet/peer"
	"github.com/davyxu/cellnet/proc"
	"proto"
	"sync"
)

type Session cellnet.Session

type ISender interface {
	Send(msg interface{})
}

type NetEvent struct {
	cellnet.Event
	Id int // message id
}

type NetMgr struct {
	tcp cellnet.Peer
	// POST队列
	handlerQue chan func()
	// 协议注册表，不加锁，网络开始前全部注册好
	handlerByIds map[int]func(User, interface{})

	userBySid map[int64]User

	eventMutex sync.Mutex
	eventQueue []NetEvent
}

func (self *NetMgr) InitNet() {
	self.handlerQue = make(chan func(), 128)
	self.handlerByIds = make(map[int]func(User, interface{}))
	self.tcp = peer.NewGenericPeer("tcp.Acceptor", "server-cat", appinfo.ServerAddr, nil)
	proc.BindProcessorHandler(self.tcp, "tcp.ltv", func(event cellnet.Event) {
		self.eventMutex.Lock()
		defer self.eventMutex.Unlock()
		msgId := cellnet.MessageToID(event.Message())
		self.eventQueue = append(self.eventQueue, NetEvent{Event: event, Id: msgId})
	})
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

func (self *NetMgr) NetChan() chan func() {
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
