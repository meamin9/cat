package user

import (
	"app/appinfo"
	"github.com/davyxu/cellnet"
	"github.com/davyxu/cellnet/peer"
	"github.com/davyxu/cellnet/proc"
)

type NetEvent interface {
	cellnet.Event
	MsgId() int
}

type RecNetEvent struct {
	cellnet.Event
	msgId int
}

func (r *RecNetEvent) MsgId() int {
	return r.msgId
}

type Network struct {
	tcp cellnet.Peer
	eventChan chan NetEvent
}

func (n *Network) Init() {
	n.eventChan = make(chan NetEvent, 256)
	//n.handlerByIds = make(map[int]func(User, interface{}))
	n.tcp = peer.NewGenericPeer("tcp.Acceptor", "server-cat", appinfo.ServerAddr, nil)
	proc.BindProcessorHandler(n.tcp, "tcp.ltv", func(event cellnet.Event) {
		msgId := cellnet.MessageToID(event.Message())
		n.eventChan <- &RecNetEvent{Event: event, msgId: msgId}
	})
}

func (n *Network) Start() {
	n.tcp.Start()
}

func (n *Network) Stop() {
	n.tcp.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
}


func (n *Network) NetEventChan() chan NetEvent {
	return n.eventChan
}

//func (n *Network) Call(handler func()) {
//	handler()
//}
//
//// 处理完队列里的消息
//func (n *Network) Flush() {
//	for {
//		select {
//		case handler := <-n.eventChan:
//			handler()
//		default:
//			break
//		}
//	}
//}

//var Instance *Network
//
//func New() *Network {
//	Instance = &Network{}
//	return Instance
//}
