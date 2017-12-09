package account

import (
	"cellnet"
	"common"
	"data"
	"db"
	"db/collections"
	"keys"
	"network"
	"proto"
	"role"
)

func init() {
	// TODO: 改成io线程并发
	network.RegisterProto("proto.CSAccountLogin", dispatchAccountLogin)
	network.RegisterProto("proto.CSAccountCreate", dispatchAccountCreate)
}

func dispatchToken(ev *cellnet.Event) {

}

func dispatchAccountLogin(ev *cellnet.Event) {
	msg := ev.Msg.(*proto.CSAccountLogin)
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return collections.AccountLogin(msg.Id, msg.Pwd)
		},
		Result: func(data interface{}, err error) {
			if err == nil {
				datas := data.([]map[string]interface{})
				ack := proto.SCRoleList{}
				roles := make([]*proto.RoleBase, len(dbroles))
				ids := make([]int64, len(dbroles))
				for i, r := range roles {
					roles[i] = &proto.RoleBase{
						Id:     r.Id,
						Name:   r.Name,
						Gender: r.Gender,
						Level:  r.Level,
					}
					ids[i] = r.Id
				}
				ack.Roles = roles
				ev.Send(&ack)
				AccountMgr().Add(ev.Ses.ID(), NewAccount(msg.Id, ids))
				return
			}
			ev.Send(common.NewNoticeMsg(keys.NoticeLoginWrongKey))
		},
	})
}

func dispatchAccountCreate(ev *cellnet.Event) {
	msg := ev.Msg.(*proto.CSAccountCreate)
	l := len(msg.Id)
	if l < data.ConstMinAccountLength || l > data.ConstMaxAccountLength {
		ev.Send(common.NewNoticeMsg(keys.NoticeLoginWrongNameLength,
			string(data.ConstMinAccountLength), string(data.ConstMaxAccountLength)))
		return
	}
	l = len(msg.Pwd)
	if l < data.ConstMinAccountLength || l > data.ConstMaxAccountLength {
		ev.Send(common.NewNoticeMsg(keys.NoticeLoginWrongPwdLength,
			string(data.ConstMinAccountLength), string(data.ConstMaxAccountLength)))
		return
	}
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, db.RetCode) {
			return collections.AccountRegister(msg.Id, msg.Pwd)
		},
		Result: func(data interface{}, code db.RetCode) {
			if code != db.CodeSuccess {
				ev.Send(common.NewNoticeMsg(keys.NoticeRegisterExist))
				return
			}
			AccountMgr().Add(ev.Ses.ID(), NewAccount(msg.Id, make([]int64, 1)))
			ack := proto.SCRoleList{}
			ack.Roles = make([]*proto.RoleBase, 0)
			ev.Send(ack)
		},
	})
}

func dispatchRoleCreate(ev *cellnet.Event) {
	msg := ev.Msg.(*proto.CSRoleCreate)
	db.Queue().Send(&Request{
		Quest: func() (interface{}, db.RetCode) {
			return collections.RoleCreate(role.RoleMgr().NextId(), msg.Name, byte(msg.Gender, byte(msg.Job)))
		},
		Result: func(data interface{}, code db.RetCode) {
			if code != db.CodeSuccess {
				ev.Send(common.NewNoticeMsg(keys.NRoleNameExist))
				return
			}
			dbrole := data.(collections.Role)
			ac, _ := AccountMgr().Get(ev.Ses.ID())
			ac.Add(dbrole.Id)
		},
	})

}
