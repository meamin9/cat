package login

import (
	"cellnet"
	"network"
	_ "proto/loginproto"
)

func init() {
	network.RegisterProto("loginproto.CSLogin", dispatchLogin)
	network.RegisterProto("loginproto.CSRoleList", dispatchRoleList)
	network.RegisterProto("loginproto.CSLoginRole", dispatchLoginRole)
}

func dispatchLogin(ev *cellnet.Event) {
}

func dispatchRoleList(ev *cellnet.Event) {
}

func dispatchLoginRole(ev *cellnet.Event) {
}
