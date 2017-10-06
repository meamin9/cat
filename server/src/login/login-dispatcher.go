package login

import (
	"cellnet"
	"network"
	_ "proto/loginproto"
)

func init() {
	var queue cellnet.EventQueue
	cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSToken", dispatchToken)
	cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSLogin", dispatchLogin)
	cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSRoleList", dispatchRoleList)
	cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSLoginRole", dispatchLoginRole)
}

func dispatchToken(ev *cellner.Event) {

}

func dispatchLogin(ev *cellnet.Event) {
	msg := ev.Msg.(loginproto.CSLogin)

}

func dispatchRoleList(ev *cellnet.Event) {
}

func dispatchLoginRole(ev *cellnet.Event) {
}
