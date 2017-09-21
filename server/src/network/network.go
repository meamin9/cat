package network

import (
	"github.com/davyxu/cellnet"
	"github.com/davyxu/cellnet/socket"
)

var host cellnet.Peer

func init() {
	queue := cellnet.NewEventQueue()
	host = socket.NewAcceptor(queue).Start("127.0.0.1:7200")
	queue.StartLoop()
}

func RegisterProto(protoName string, userCallback func(*cellnet.Event)) {
	cellnet.RegisterMessage(host, protoName, userCallback)
}
