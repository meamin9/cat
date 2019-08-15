package user

import (
	"app/consts"
	"app/db"
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

func init() {
	Mgr.RegNetMsg(proto.KeyAccountCreate, accountCreate)
	Mgr.RegNetMsg(proto.KeyAccountLogin, accountLogin)
}

func accountCreate(user User, netMsg interface{}) {
	msg := netMsg.(proto.AccountCreate)
	req := msg.Req
	msg.Req = nil
	id := strings.TrimSpace(req.Id)
	pwd := strings.TrimSpace(req.Pwd)
	if !consts.AccountLen.In(len(id)) {
		msg.Err = 1
		goto ERROR
	}
	if !consts.AccountLen.In(len(pwd)) {
		msg.Err = 2
		goto ERROR
	}
	db.Instance.Send(&db.dbEvent{
		Sql: &db.SqlAccountCreate{id, pwd},
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
ERROR:
	user.Send(msg)
}

func accountLogin(user User, msg interface{}) {
	ERROR:
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
	db.Instance.Send(&db.dbEvent{
		Sql: &db.SqlAccountCreate{id, pwd},
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
	db.Instance.Send(&db.dbEvent{
		Sql: &db.SqlAccountLogin{id, pwd},
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
