package role

import (
	"app/network"
	"proto"
)

func regProp() {
	network.Instance.RegProto(proto.CodeCSRoleCreate, recvRoleCreate)
	//network.Instance.RegProto(proto.CodeCSRoleEnter, csAccountLogin)
}

func recvRoleCreate(ses network.Session, data interface{}) {
	msg := data.(*proto.CSRoleCreate)
}

