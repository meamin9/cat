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
	*Cfg
}

func (self *AccountSvc) Start() {
	network.Svc.RegProto(proto.CodeCSAccountCreate, self.csAccountCreate)
	network.Svc.RegProto(proto.CodeCSAccountLogin, self.csAccountLogin)
}

func (self *AccountSvc) csAccountLogin(ses network.Session, imsg interface{}) {
	msg := imsg.(*proto.CSAccountLogin)
	namelen := len(msg.Id)
	if namelen < self.Cfg.NameLenRange[0] || namelen > self.Cfg.NameLenRange[1] {
		return
	}
	pwdlen := len(msg.Pwd)
	if pwdlen < self.Cfg.PwdLenRange[0] || pwdlen > self.Cfg.PwdLenRange[1] {
		return
	}
	db.Svc.Send(&db.Mail{
		Sql: &dbAccountRegister{msg.Id, msg.Pwd},
		Cb: func(data interface{}, err error) {
			if err != nil {
				ses.Send(common.NewNoticeMsg(keys.NoticeLoginWrongKey))
				return
			}
		},
	})
}

func (self *AccountSvc) csAccountCreate(ses network.Session, imsg interface{}) {
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
