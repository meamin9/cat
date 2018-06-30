package network

import (
	"cellnet"
	"cellnet/socket"
	"app/service"
)


type Network struct {
	service.ServiceBase
	host cellnet.Peer
}

func NewNetWork() service.IService {
	return &Network{
		ServiceBase: service.NewServiceBase("NewNetWork"),

	}
}

func (self *Network) Install() {
	self.ServiceBase.Install()
	self.host = socket.NewAcceptor(cellnet.NewEventQueue())
	self.host.Start("127.0.0.1:7200")
}

var peer cellnet.Peer
var queue cellnet.EventQueue

func init() {
	queue = cellnet.NewEventQueue()
	peer = socket.NewAcceptor(queue)
}

func RegisterProto(protoName string, userCallback func(*cellnet.Event)) {
	cellnet.RegisterMessage(peer, protoName, userCallback)
}

func RegisterMessage(protoName string, userCallback func(*cellnet.Event)) {
	cellnet.RegisterMessage(peer, protoName, userCallback)
}

func Peer() cellnet.Peer {
	return peer
}

func Queue() cellnet.EventQueue {
	return queue
}

func Start() {
	peer.Start("127.0.0.1:7200")
}
