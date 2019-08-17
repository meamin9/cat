package user

import (
	"app/db"
	"app/fw/consts"
	"app/mosaic"
	"app/notice"
	"proto"
	"strings"
)

//var cryptoKey = "milan, milan go!"
//
//type entryToken struct {
//	Token   string
//	Timeout *time.Timer
//	Ok      bool // 合法的连接这个值要为true
//}
//

func accountCreate(user User, netMsg interface{}) {
	msg := netMsg.(proto.AccountCreate)
	req := msg.Req
	msg.Req = nil
	id := strings.TrimSpace(req.Id)
	pwd := strings.TrimSpace(req.Pwd)
	idLen := len(id)
	if idLen < consts.AccountMinLen || consts.AccountMaxLen < idLen {
		msg.Err = consts.ErrAccountIdLen
		goto ERROR
	}
	if idLen < consts.AccountMinLen || consts.AccountMaxLen < idLen {
		msg.Err = consts.ErrAccountPwdLen
		goto ERROR
	}
	db.Manager.Send(&db.SqlAccountCreate{Id: id, Pwd: pwd}, func(data interface{}, err error) {
		if err != nil { // 注册失败，可能是用户名已存在
			msg.Err = consts.ErrAccountExist
			user.Send(msg)
			return
		}
		acc := &Account{
			Id:    id,
			Roles: make([]*mosaic.RoleInfo, 0),
		}
		user.account = acc
		user.Send(msg)
	})
ERROR:
	user.Send(msg)
}

func accountLogin(user User, netMsg interface{}) {
	msg := netMsg.(proto.AccountLogin)
	req := msg.Req
	msg.Req = nil
	db.Manager.Send(&db.SqlAccountLogin{Id:req.Id, Pwd:req.Pwd}, func(data interface{}, err error) {
		if err != nil {
			msg.Err = consts.ErrRoleLogin
			user.Send(msg)
			return
		}
		roleinfo := proto.RoleInfo_{}
		rsp := &proto.AccountLogin_Rsp{

		}

	})
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

func init() {
	Manager.RegNetMsg(proto.KeyAccountCreate, accountCreate)
	Manager.RegNetMsg(proto.KeyAccountLogin, accountLogin)
}