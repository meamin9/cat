package user

import (
	"app/fw/appinfo"
	"app/fw/apptime"
	"github.com/davyxu/cellnet"
	"github.com/davyxu/cellnet/peer"
	"github.com/davyxu/cellnet/proc"
)

type RPCMsg struct {
	SessionId uint16
	Message interface{}
}

type network struct {
	tcp cellnet.Peer
	eventChan chan func()
	procByNetMsgId map[int]func(User, interface{})
	userBySid      map[int64]User
}

func newNetWork() (n *network) {
	n = &network{
		userBySid:      make(map[int64]User),
		procByNetMsgId: make(map[int]func(User, interface{})),
		eventChan: make(chan func(), 256),
		tcp: peer.NewGenericPeer("tcp.Acceptor", "server-cat", appinfo.ServerAddr, nil),
	}
	proc.BindProcessorHandler(n.tcp, "tcp.ltv", func(event cellnet.Event) {
		msgId := cellnet.MessageToID(event.Message())
		n.eventChan <- func() {
			n.ProcNetEvent(event, msgId)
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

func (n *network) ProcNetEvent(event cellnet.Event, msgId int) {
	defer func() {
		if err := recover(); err != nil {
			log.Errorf("panic:%+v", err)
		}
	}()
	ses := event.Session()
	msg := event.Message()
	switch msg.(type) {
	default:
		if user, ok := n.userBySid[ses.ID()]; ok {
			if proc, ok := n.procByNetMsgId[msgId]; ok {
				proc(user, event.Message())
			} else {
				log.Warnf("unknown net msgId=%v", msgId)
			}
		} else {
			log.Errorf("not found user sessionId=%v, msgId=%v", ses.ID(), msgId)
			ses.Close()
		}
	case cellnet.SessionAccepted:
		log.Debugf("session accepted sid=%v time=%v", ses.ID(), apptime.Now())
		n.userBySid[ses.ID()] = &implementUser{Session: ses, ctime: apptime.Now()}
	case cellnet.SessionClosed:
		if user, ok := n.userBySid[ses.ID()]; ok {
			log.Warnf("session closed user=%v", user)
			ses.Close()
		} else {
			log.Errorf("not found user sessionId=%v, msgId=%v", ses.ID(), msgId)
			ses.Close()
		}
	}
}

func (n *network) RegNetMsg(msgId int, proc func(User, interface{})) {
	if _, ok := n.procByNetMsgId[msgId]; ok {
		panic("proto is register repeated")
	}
	n.procByNetMsgId[msgId] = proc
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

