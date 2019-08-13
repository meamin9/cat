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

type Network struct {
	tcp cellnet.Peer
	// POST队列
	handlerQue chan func()
	// 协议注册表，不加锁，网络开始前全部注册好
	handlerByIds map[int]func(User, interface{})

	userBySid map[int64]User

	eventMutex sync.Mutex
	eventQueue []NetEvent

	eventChan chan NetEvent
}

func (n *Network) InitNet() {
	n.eventChan = make(chan NetEvent, 256)
	//n.handlerByIds = make(map[int]func(User, interface{}))
	n.tcp = peer.NewGenericPeer("tcp.Acceptor", "server-cat", appinfo.ServerAddr, nil)
	proc.BindProcessorHandler(n.tcp, "tcp.ltv", func(event cellnet.Event) {
		msgId := cellnet.MessageToID(event.Message())
		n.eventChan <- NetEvent{Event: event, Id: msgId}
	})
}

func (n *Network) Start() {
	n.tcp.Start()
}

func (n *Network) Stop() {
	n.tcp.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
	// n := n.host.(interface{ Count() int }).Count()
	//n.exitSync.Add(n)
}

func (n *Network) RegProto(msgId int, cb func(Session, interface{})) {
	if _, ok := n.handlerByIds[msgId]; ok {
		panic("proto is register repeated")
	}
	n.handlerByIds[msgId] = cb
}

func (n *Network) UnregProto(key int) {
	delete(n.handlerByIds, key)
}

func (n *Network) NetChan() chan func() {
	return n.handlerQue
}

func (n *Network) Call(handler func()) {
	handler()
}

// 处理完队列里的消息
func (n *Network) Flush() {
	for {
		select {
		case handler := <-n.handlerQue:
			handler()
		default:
			break
		}
	}
}

var Instance *Network

func New() *Network {
	Instance = &Network{}
	return Instance
}
