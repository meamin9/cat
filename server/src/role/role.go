package role

import (
	"base"
	"network"
)

type SexType byte

const (
	Female SexType = iota
	Male
)

type RoleFixedProp struct {
	id   base.RoleId
	name string
	sex  SexType
}

type RoleBaseProp struct {
	level int
}

type Role struct {
	RoleFixedProp
	RoleBaseProp
	sid int64
}

func (self *Role) Id() base.RoleId {
	return self.id
}

func (self *Role) Name() string {
	return self.name
}

func (self *Role) Send(data interface{}) {
	session := network.Host.GetSession(self.sid)
	if session != nil {
		session.Send(data)
	}
}
