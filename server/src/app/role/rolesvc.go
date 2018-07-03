package role

import (
	"app"
	"app/network"
	"proto"
)

type RoleSvc struct {
	app.ServiceBase
	*RoleMgr
}

func (self *RoleSvc) Init() {
	network.Instance.RegProto(proto.CodeCSRoleCreate, self.csRoleCreate)
}

func (self *RoleSvc) csRoleCreate(ses network.Session, data interface{}) {
	msg := data.(*proto.CSRoleCreate)

}
