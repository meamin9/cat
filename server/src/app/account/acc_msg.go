package account

import (
	"proto"
	"app/db"
	"app/network"
	"app/notice"
	"strings"
	"app/role"
	"regexp"
)

func regProp() {
	network.Instance.RegProto(proto.CodeCSAccountReg, recvAccountReg)
	network.Instance.RegProto(proto.CodeCSAccountLogin, recvAccountLogin)
}

func checkValidity(id, pwd string) bool {
	l := len(id)
	if l < Instance.NameLenRange[0] || l > Instance.NameLenRange[1] {
		return false
	}
	if ok, err := regexp.MatchString("[0-9a-zA-Z_-]+", id);
	err != nil || !ok{
		return false
	}
	l = len(pwd)
	if l < Instance.PwdLenRange[0] || l > Instance.PwdLenRange[1] {
		return false
	}
	return true
}

func sendAccountInfo(sender network.ISender, account *Account) {
	msg := &proto.SCAccountInfo{
		Id: account.Id,
	}
	infos := make([]*proto.RoleInfo, len(account.Roles))
	for i, info := range account.Roles {
		infos[i] = info.PackMsg()
	}
	msg.Roles = infos
	sender.Send(msg)
}

func recvAccountReg(ses network.Session, data interface{}) {
	msg := data.(*proto.CSAccountReg)
	id := strings.TrimSpace(msg.Id)
	pwd := strings.TrimSpace(msg.Pwd) // 这是个md5码值
	if ! checkValidity(id, pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &dbAccountCreate{id, pwd},
		Cb: func(data interface{}, err error) {
			if err != nil { // 注册失败，可能是用户名已存在
				notice.SendNotice(ses, notice.CNameRepeated)
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
	if ! checkValidity(id, pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &dbAccountLogin{id, pwd},
		Cb: func(data interface{}, err error) {
			if err != nil {
				notice.SendNotice(ses, notice.CLoginInvalid)
				return
			}
			acc := data.(*Account)
			Instance.AddAccount(acc)
			sendAccountInfo(ses, acc)
		},
	})
}
