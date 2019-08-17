package user

import (
	"app/db"
	"app/fw/consts"
	"app/fw/util"
	"proto"
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
	id, pwd := req.Id, req.Pwd
	//id := strings.TrimSpace(req.Id)
	//pwd := strings.TrimSpace(req.Pwd)
	if !util.IsAsciiName(id) {
		msg.Err = consts.ErrAccountId
		user.Send(msg)
		return
	}
	idLen := len(id)
	if idLen < consts.AccountMinLen || consts.AccountMaxLen < idLen {
		msg.Err = consts.ErrAccountIdLen
		user.Send(msg)
		return
	}
	if idLen < consts.AccountMinLen || consts.AccountMaxLen < idLen {
		msg.Err = consts.ErrAccountPwdLen
		user.Send(msg)
		return
	}
	db.Manager.Send(&db.SqlAccountCreate{Id: id, Pwd: pwd}, func(data interface{}, err error) {
		if err != nil { // 注册失败，可能是用户名已存在
			msg.Err = consts.ErrAccountExist
			user.Send(msg)
			return
		}
		user.account = id
		user.Send(msg)
	})
}

func accountLogin(user User, netMsg interface{}) {
	msg := netMsg.(proto.AccountLogin)
	req := msg.Req
	msg.Req = nil
	id := req.Id
	db.Manager.Send(&db.SqlAccountLogin{Id:req.Id, Pwd:req.Pwd}, func(data interface{}, err error) {
		if err != nil {
			msg.Err = consts.ErrRoleLogin
			user.Send(msg)
			return
		}
		roles := data.([]*db.RoleLoginInfo)
		rsp := &proto.AccountLogin_Rsp{}
		for _, r := range roles {
			rsp.Roles = append(rsp.Roles, &proto.RoleInfo_{
				Id: r.Id,
				Name: r.Name,
				Gender: int32(r.Gender),
				Job: int32(r.Job),
				Level: int32(r.Level),
				LogoutTime: r.LogoutTime.Unix(),
			})
		}
		msg.Rsp = rsp
		user.account = id
		user.Send(msg)
	})
}

func roleCreate(user User, netMsg interface{}) {
	msg := netMsg.(proto.RoleCreate)
	req := msg.Req
	msg.Req = nil
	name, gender, job := req.Name, int(req.Gender), int(req.Job)
	if !util.IsUnicodeName(name) {
		msg.Err = consts.ErrRoleNameInvalid
		user.Send(msg)
		return
	}
	nameLen := len(name)
	if nameLen < consts.RoleNameMinLen || nameLen > consts.RoleNameMaxLen {
		msg.Err = consts.ErrRoleNameLen
		user.Send(msg)
		return
	}
	id := newRoleId()
	db.Manager.Send(&db.SqlCreateRole{
		Id:id, Name:name, Gender:gender, Job:job, Account:user.account,
	}, func(ret interface{}, err error) {
		if err != nil {
			msg.Err = consts.ErrRoleNameExist
			user.Send(msg)
			return
		}
		user.Send(msg)
	})
}

func init() {
	Manager.RegNetMsg(proto.KeyAccountCreate, accountCreate)
	Manager.RegNetMsg(proto.KeyAccountLogin, accountLogin)
	Manager.RegNetMsg(proto.KeyRoleCreate, roleCreate)
}