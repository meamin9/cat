package role

import (
	"time"
	"app/network"
	"proto"
)

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

// 打包到网络发送
func (self *RoleInfo) PackMsg() *proto.RoleInfo {
	return &proto.RoleInfo{
		Id: self.Id,
		Name: self.Name,
		LogoutTime: self.LogoutTime.Unix(),
		Gender: int32(self.Gender),
		Level: int32(self.Level),
		Job: int32(self.Job),
	}
}


type Role struct {
	*RoleInfo
	CTime time.Time
	Account string
	Session network.Session
}

func newRole(name string, gender EGender, job EJob) *Role {
	return &Role{
		RoleInfo: &RoleInfo{
			Id: Instance.NewId(),
			Name: name,
			Gender: gender,
			Job: job,
			Level: 1,
			LogoutTime: time.Unix(0, 0),
		},
		CTime: time.Now(),
	}
}

func (self *Role) Pack() *DbRole{
	return &DbRole{
		Id: self.Id,
		Name: self.Name,
		Gender: int(self.Gender),
		Job: int(self.Job),
		Level: self.Level,
		LogoutTime: self.LogoutTime,
		CTime: self.CTime,
		Account: self.Account,
	}
}

