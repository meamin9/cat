package account

import (
	"proto"
	"app/common"
	"app/keys"
	"app/db"
	"app/db/collections"
	"app/data"
	"log"
	"app"
	"app/network"
)

type AccountSvc struct {
	app.ServiceBase
}

func Install() {
	app.NetSvc().RegProto(proto.CodeCSAccountCreate, csAccountCreate)
	network.RegisterProto("proto.CSAccountLogin", dispatchAccountLogin)
	network.RegisterProto("proto.CSAccountCreate", dispatchAccountCreate)
}

func dispatchToken(ev *cellnet.Event) {

}

func dispatchAccountLogin(ev *cellnet.Event) {
	msg := ev.Msg.(*proto.CSAccountLogin)
	account, ok := Mgr().Account(msg.Id)
	if ok == true {
		if account.pwd != msg.Pwd {
			ev.Send(common.NewNoticeMsg(keys.NoticeLoginWrongKey))
			return
		}
		account.addSid(ev.Ses.ID())
		ev.Ses.SetAccountId(msg.Id)
		ev.Send(account.data())
		return
	}
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return collections.AccountLogin(msg.Id, msg.Pwd)
		},
		Result: func(data interface{}, err error) {
			if err != nil {
				ev.Send(common.NewNoticeMsg(keys.NoticeLoginWrongKey))
				return
			}
			datas := data.([]map[string]interface{})
			account := newAccount(ev.Ses.ID(), msg.Id, msg.Pwd)
			account.unpackRoles(datas)
			Mgr().Add(account)
			ev.Ses.SetAccountId(msg.Id)
			ev.Send(account.data())
		},
	})
}

func csAccountCreate(ses network.Session, imsg interface{}) {
	msg := imsg.(*proto.CSAccountCreate)
	l := len(msg.Id)
	if l < data.ConstMinAccountLength || l > data.ConstMaxAccountLength {
		ses.Send(common.NewNoticeMsg(keys.NoticeLoginWrongNameLength,
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
		Quest: func() (interface{}, error) {
			return collections.AccountRegister(msg.Id, msg.Pwd)
		},
		Result: func(data interface{}, err error) {
			if err != nil {
				ev.Send(common.NewNoticeMsg(keys.NoticeRegisterExist))
				return
			}
			acc := newAccount(ev.Ses.ID(), msg.Id, msg.Pwd)
			Mgr().Add(acc)
			ev.Ses.SetAccountId(msg.Id)
			ev.Send(acc.data())
		},
	})
}

func dispatchRoleCreate(ev *cellnet.Event) {
	msg := ev.Msg.(*proto.CSRoleCreate)
	aid := ev.Ses.AccountId()
	acc, ok := Mgr().Account(aid)
	if !ok {
		log.Panicln("invaild account", aid)
	}
	if len(acc.roles) >= 3 {
		log.Print("create roles reach max count", acc)
		return
	}
	role := role.Mgr().CreateRole(msg.Name)
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return collections.RoleCreate(role.Pack())
		},
		Result: func(data interface{}, err error) {
			if err != nil {
				ev.Send(common.NewNoticeMsg(keys.NRoleNameExist))
				return
			}
			acc.addRole(&roleinfo{
				id:    role.Id(),
				name:  role.Name(),
				level: role.Level(),
			})
			db.Queue().Send(db.NewRequest(func() (interface{}, error) {
				return collections.AccountUpdateRoles(acc.packRoles())
			}, nil))
		},
	})

}
