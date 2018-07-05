package role

import "time"

type EGender int
const (
	Female EGender = iota
	Male
)

type EJob int
const (
	None EJob = iota
)

type RoleInfo struct {
	Id     uint64
	Name   string
	Gender EGender
	Job EJob
	Level int
	LogoutTime time.Time
}

type Role struct {
	RoleInfo
	CTime time.Time
	Prop *CProp
}



func NewRole(id uint64, name string) *Role {
	return &Role{
		Name: name,
		Id: id,
	}
}

//func (me *Role) Pack() map[string]interface{} {
//	m := make(map[string]interface{}, 10)
//	m["_id"] = me.id
//	m["name"] = me.name
//	return m
//}
//
//func (me *Role) Unpack(m map[string]interface{}) {
//	me.id = m["_id"]
//	me.name = m["name"]
//}
//

