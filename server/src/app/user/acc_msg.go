package user

import (
	"app/db"
	"app/db/collection"
	"app/mosaic"
	"app/notice"
	"app/util"
	"github.com/davyxu/cellnet/util"
	"proto"
	"strings"
	"time"
)

var cryptoKey = "milan, milan go!"

type entryToken struct {
	Token   string
	Timeout *time.Timer
	Ok      bool // 合法的连接这个值要为true
}

var (
	CodeSessionAccepted = int(util.StringHash("cellnet.SessionAccepted"))
	CodeSessionClosed   = int(util.StringHash("cellnet.SessionClosed"))
)

func regProp() {
	m := proto.RPCCreate{
		Req: &proto.RPCCreate_Req{},
	}

	Instance.RegProto()
	Instance.RegProto(proto.KeyCSAccountReg, recvAccountReg)
	Instance.RegProto(proto.KeyCSAccountLogin, recvAccountLogin)
}

func checkValidity(id, pwd string) bool {
	return mosaic.Const.AccountNameLengthRange.InRange(len(id)) &&
		util.IsWords(id) &&
		len(pwd) > 0
}

func sendAccountInfo(sender ISender, account *Account) {
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

func recvAccountReg(ses Session, data interface{}) {
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

func recvAccountLogin(ses Session, data interface{}) {
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
