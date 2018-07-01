package role

import (

)

type Role struct {
	id     int64
	name   string
}

func newRole() *Role {
	return &Role{}
}

func (self *Role) Id() int64 {
	return self.id
}

func (self *Role) Name() string {
	return self.name
}

func (me *Role) Pack() map[string]interface{} {
	m := make(map[string]interface{}, 10)
	m["_id"] = me.id
	m["name"] = me.name
	return m
}

func (me *Role) Unpack(m map[string]interface{}) {
	me.id = m["_id"]
	me.name = m["name"]
}


