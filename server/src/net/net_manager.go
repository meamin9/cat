package net

import (
	"github.com/davyxu/cellnet"
	"github.com/davyxu/cellnet/socket"
)

var serv cellnet.Peer

func Initialize() {
	queue := cellnet.NewEventQueue()
	serv = socket.NewAcceptor(queue).Start("127.0.0.1:7200")
	queue.StartLoop()
}

//func RegisterProto(protoName string, response ())
