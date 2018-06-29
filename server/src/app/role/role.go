package role

import (
	"common/class"
	"time"
)

type Role struct {
	sid    int64
	id     int64
	name   string
	gender class.Gender
	level  int16
	birth  time.Date
}

func (self *Role) Id() int64 {
	return self.id
}

func (self *Role) Name() string {
	return self.name
}

func (self *Role) Send(data interface{}) {
}

func (me *Role) Pack() map[string]interface{} {
	m := make(map[string]interface{}, 10)
	m["_id"] = me.id
	m["name"] = me.name
	m["gender"] = me.gender
	m["level"] = me.level
	m["birth"] = me.birth
	return m
}

func (me *Role) Unpack(m map[string]interface{}) {
	me.id = m["_id"]
	me.name = m["name"]
	me.gender = m["gender"]
	me.level = m["level"]
	me.birth = m["birth"]
}

func newRole() *Role {
	return &Role{}
}
