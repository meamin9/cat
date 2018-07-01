package network

import (
	"cellnet"
)

// 网络消息处理

type ProtoHander func(session Session, msg interface{})

type dispatcher struct {
	protos map[int]func(Session, interface{})
}

func (self *dispatcher) RegProto(key int, handler func(Session, interface{})) {
	if _, ok := self.protos[key]; ok {
		panic("proto is register repeated")
	}
	self.protos[key] = handler
}

func (self *dispatcher) UnregProto(key int) {
	delete(self.protos, key)
}

func (self *dispatcher) ProcProto(event cellnet.Event) {
	ses := event.Session().(Session)
	msg := event.Message()
	if handler, ok := self.protos[cellnet.MessageToID(msg)]; ok {
		handler(ses, msg)
	}
}
