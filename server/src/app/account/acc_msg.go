package account

import (
	"app/db"
	"app/db/collection"
	"app/mosaic"
	"app/network"
	"app/notice"
	"proto"
	"strings"
	"app/util"
)

func regProp() {
	network.Instance.RegProto(proto.CodeCSAccountReg, recvAccountReg)
	network.Instance.RegProto(proto.CodeCSAccountLogin, recvAccountLogin)
}

func checkValidity(id, pwd string) bool {
	return mosaic.Const.AccountNameLengthRange.InRange(len(id)) &&
		util.IsWords(id) &&
		len(pwd) > 0
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
	if !checkValidity(id, pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &collection.SqlAccountCreate{id, pwd},
		Cb: func(data interface{}, err error) {
			if err != nil { // 注册失败，可能是用户名已存在
				notice.SendNotice(ses, notice.CNameRepeated)
				return
			}
			acc := &Account{
				Id:    id,
				Roles: make([]*mosaic.RoleInfo, 0),
			}
			Instance.AddLoginAccount(acc, ses.ID())
			sendAccountInfo(ses, acc)
		},
	})
}

func recvAccountLogin(ses network.Session, data interface{}) {
	msg := data.(*proto.CSAccountLogin)
	id := strings.TrimSpace(msg.Id)
	pwd := strings.TrimSpace(msg.Pwd) // 这是个md5码值
	if !checkValidity(id, pwd) {
		return
	}
	db.Instance.Send(&db.Mail{
		Sql: &collection.SqlAccountLogin{id, pwd},
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
