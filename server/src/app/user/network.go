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

type network struct {
	tcp cellnet.Peer
	eventChan chan func()
}

func newNetWork() (n *network) {
	n = &network{
		eventChan: make(chan func(), 256),
		tcp: peer.NewGenericPeer("tcp.Acceptor", "server-cat", appinfo.ServerAddr, nil),
	}
	proc.BindProcessorHandler(n.tcp, "tcp.ltv", func(event cellnet.Event) {
		msgId := cellnet.MessageToID(event.Message())
		n.eventChan <- func() {
			Manager.ProcNetEvent(event, msgId)
		}
	})
	return
}

func (n *network) Start() {
	n.tcp.Start()
}

func (n *network) Stop() {
	n.tcp.Stop() // 阻塞直到accept线程退出，ses的工作线程不一定全退出了
}


func (n *network) EventChan() chan func() {
	return n.eventChan
}

//func (n *network) Call(handler func()) {
//	handler()
//}
//
//// 处理完队列里的消息
//func (n *network) Flush() {
//	for {
//		select {
//		case handler := <-n.eventChan:
//			handler()
//		default:
//			break
//		}
//	}
//}

//var Instance *network
//
//func New() *network {
//	Instance = &network{}
//	return Instance
//}
