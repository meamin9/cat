package login

import (
	"cellnet"
	"db"
	"db/collections"
	"keys"
	"network"
	"proto"
)

func init() {
	// TODO: 改成io线程并发
	network.RegisterProto("proto.CSAccountlogin", dispatchLogin)
	// var queue cellnet.EventQueue
	// cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSToken", dispatchToken)
	// cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSLogin", dispatchLogin)
	// cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSRoleList", dispatchRoleList)
	// cellnet.RegisterMessageToQueue(network.Peer, queue, "loginproto.CSLoginRole", dispatchLoginRole)
}

func dispatchToken(ev *cellnet.Event) {

}

func dispatchLogin(ev *cellnet.Event) {
	msg := ev.Msg.(proto.CSAccountLogin)
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, db.RetCode) {
			return collections.AccountLogin(msg.Id, msg.Pwd)
		},
		Result: func(data interface{}, code db.RetCode) {
			if code == db.CodeSuccess {
				dbroles := data.([]collections.RoleBase)
				msg := proto.SCRoleList{}
				roles := make([]*proto.RoleBase, len(dbroles))
				for i, r := range roles {
					roles[i] = &proto.RoleBase{
						Id:     r.Id,
						Name:   r.Name,
						Gender: r.Gender,
						Level:  r.Level,
					}
				}
				msg.Roles = roles
				ev.Send(&roles)
				return
			}
			ev.Send(&proto.SCNotice{Index: keys.NoticeLoginWrongKey})
		},
	})
}

func dispatchRoleList(ev *cellnet.Event) {
}

func dispatchLoginRole(ev *cellnet.Event) {
}
