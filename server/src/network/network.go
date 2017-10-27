package network

import (
	"cellnet"
	"cellnet/socket"
)

var Peer cellnet.Peer
var queue cellnet.EventQueue

func init() {
	queue = cellnet.NewEventQueue()
	Peer = socket.NewAcceptor(queue).Start("127.0.0.1:7200")
}

func RegisterProto(protoName string, userCallback func(*cellnet.Event)) {
	cellnet.RegisterMessage(Peer, protoName, userCallback)
}

func RegisterMessage(protoName string, userCallback func(*cellnet.Event)) {
	cellnet.RegisterMessage(Peer, protoName, userCallback)
}

func Queue() cellnet.EventQueue {
	return queue
}
