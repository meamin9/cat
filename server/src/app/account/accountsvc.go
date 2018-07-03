package account

import (
	"proto"
	"app/common"
	"app/keys"
	"app/db"
	"app/db/collections"
	"log"
	"app"
	"app/network"
	"app/notice"
)

type AccountSvc struct {
	app.ServiceBase
	*Cfg
}

func (self *AccountSvc) Init() {
	network.Instance.RegProto(proto.CodeCSAccountReg, self.csAccountReg)
	network.Instance.RegProto(proto.CodeCSAccountLogin, self.csAccountLogin)
}

func (self *AccountSvc) Start() {}
func (self *AccountSvc) Stop() {}

func (self *AccountSvc) CheckNamePwdVaild(name, pwd string) bool {
	l := len(name)
	if l < self.Cfg.NameLenRange[0] || l > self.Cfg.NameLenRange[1] {
		return false
	}
	l = len(pwd)
	if l < self.Cfg.PwdLenRange[0] || l > self.Cfg.PwdLenRange[1] {
		return false
	}
	return true
}

func (self *AccountSvc) csAccountReg(ses network.Session, imsg interface{}) {
	msg := imsg.(*proto.CSAccountReg)
	if ! self.CheckNamePwdVaild(msg.Id, msg.Pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &dbAccountRegister{msg.Id, msg.Pwd},
		Cb: func(data interface{}, err error) {
			if err != nil { // 注册失败，可能是用户名已存在
				notice.SendText(ses, "用户名已存在")
				return
			}
			//d := data.(map[string]interface{})
			//acc := newAccount(ev.Ses.ID(), msg.Id, msg.Pwd)
			//Mgr().Add(acc)
			//ev.Ses.SetAccountId(msg.Id)
			//ev.Send(acc.data())
		},
	})
}

func (self *AccountSvc) csAccountLogin(ses network.Session, imsg interface{}) {
	msg := imsg.(*proto.CSAccountLogin)
	if ! self.CheckNamePwdVaild(msg.Id, msg.Pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &dbAccountRegister{msg.Id, msg.Pwd},
		Cb: func(data interface{}, err error) {
			if err != nil {
				ses.Send(common.NewNoticeMsg(keys.NoticeLoginWrongKey))
				return
			}
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
