package network

import (
	"cellnet"
	"cellnet/socket"
)

var Host cellnet.Peer

func init() {
	queue := cellnet.NewEventQueue()
	Host = socket.NewAcceptor(queue).Start("127.0.0.1:7200")
	queue.StartLoop()
}

func RegisterProto(protoName string, userCallback func(*cellnet.Event)) {
	cellnet.RegisterMessage(Host, protoName, userCallback)
}
