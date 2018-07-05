package account

import (
	"proto"
	"app/db"
	"app"
	"app/network"
	"app/notice"
	"strings"
	"app/role"
)

func regProp() {
	network.Instance.RegProto(proto.CodeCSAccountReg, recvAccountReg)
	network.Instance.RegProto(proto.CodeCSAccountLogin, recvAccountLogin)
}

func checkValidity(ses network.Session, id, pwd string) bool {
	l := len(id)
	if l < Instance.NameLenRange[0] || l > Instance.NameLenRange[1] {
		return false
	}
	l = len(pwd)
	if l < Instance.PwdLenRange[0] || l > Instance.PwdLenRange[1] {
		return false
	}
	return true
}

func sendAccountInfo(sender app.ISender, account *Account) {
	msg := &proto.SCAccountInfo{
		Id: account.Id,
	}
	infos := make([]*proto.RoleInfo, len(account.Roles))
	for i, info := range account.Roles {
		infos[i] = &proto.RoleInfo{
			Id: info.Id,
			Name: info.Name,
			MTime: info.MTime.Unix(),
			Gender: int32(info.Gender),
			Level: int32(info.Level),
			Job: int32(info.Job),
		}
	}
	msg.Roles = infos
	sender.Send(msg)
}

func recvAccountReg(ses network.Session, data interface{}) {
	msg := data.(*proto.CSAccountReg)
	id := strings.TrimSpace(msg.Id)
	pwd := strings.TrimSpace(msg.Pwd) // 这是个md5码值
	if ! checkValidity(ses, id, pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &dbAccountCreate{id, pwd},
		Cb: func(data interface{}, err error) {
			if err != nil { // 注册失败，可能是用户名已存在
				notice.SendText(ses, "用户名已存在")
				return
			}
			acc := &Account{
				Id: id,
				Roles: make([]*role.RoleInfo, 0),
			}
			Instance.AddAccount(acc)
			sendAccountInfo(ses, acc)
		},
	})
}

func recvAccountLogin(ses network.Session, data interface{}) {
	msg := data.(*proto.CSAccountLogin)
	id := strings.TrimSpace(msg.Id)
	pwd := strings.TrimSpace(msg.Pwd) // 这是个md5码值
	if ! checkValidity(ses, id, pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &dbAccountLogin{id, pwd},
		Cb: func(data interface{}, err error) {
			if err != nil {
				notice.SendText(ses, "帐号或密码错误")
				return
			}
			acc := data.(*Account)
			Instance.AddAccount(acc)
			sendAccountInfo(ses, acc)
		},
	})
}


//
//func dispatchRoleCreate(ev *cellnet.Event) {
//	msg := ev.Msg.(*proto.CSRoleCreate)
//	aid := ev.Ses.AccountId()
//	acc, ok := Mgr().Account(aid)
//	if !ok {
//		log.Panicln("invaild account", aid)
//	}
//	if len(acc.roles) >= 3 {
//		log.Print("create roles reach max count", acc)
//		return
//	}
//	role := role.Mgr().CreateRole(msg.Name)
//	db.Queue().Send(&db.Request{
//		Quest: func() (interface{}, error) {
//			return collections.RoleCreate(role.Pack())
//		},
//		Result: func(data interface{}, err error) {
//			if err != nil {
//				ev.Send(common.NewNoticeMsg(keys.NRoleNameExist))
//				return
//			}
//			acc.addRole(&roleinfo{
//				id:    role.Id(),
//				name:  role.Name(),
//				level: role.Level(),
//			})
//			db.Queue().Send(db.NewRequest(func() (interface{}, error) {
//				return collections.AccountUpdateRoles(acc.packRoles())
//			}, nil))
//		},
//	})
//
//}
