package role

import (
	"app/account"
	"app/db"
	"app/db/collection"
	"app/mosaic"
	"app/network"
	"app/notice"
	"proto"
	"strings"
)

func regProp() {
	network.Instance.RegProto(proto.CodeCSRoleCreate, recvRoleCreate)
	//network.Instance.RegProto(proto.CodeCSRoleEnter, csAccountLogin)
}

func recvRoleCreate(s network.Session, data interface{}) {
	msg := data.(*proto.CSRoleCreate)
	name := strings.TrimSpace(msg.Name)
	gender := mosaic.EGender(msg.Gender)
	job := mosaic.EJob(msg.Job)
	//TODO: 检查参数合法
	acc := account.Instance.AccountBySid(s.ID())
	if acc == nil {
		// Error
		return
	}
	role := newRole(name, gender, job)
	role.Account = acc.Id
	role.Session = s
	dbrole := role.Pack()
	db.Instance.Send(&db.Mail{
		Sql: &collection.SqlRoleCreate{Role: dbrole},
		Cb: func(i interface{}, e error) {
			if e != nil {
				notice.SendNotice(s, notice.CNameRepeated)
				return
			}
			Instance.AddRole(role)
			acc.AddRoleInfo(role.RoleInfo)
			sendRoleCreate(s, role.RoleInfo)
		},
	})
}

func sendRoleCreate(s network.Session, info *mosaic.RoleInfo) {
	s.Send(info.PackMsg())
}
