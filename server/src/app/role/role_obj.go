package role

import (
	"app/db/collection"
	"app/mosaic"
	"app/network"
	"time"
)

type Role struct {
	*mosaic.RoleInfo
	CTime   time.Time
	Account string
	Session network.Session
}

func newRole(name string, gender mosaic.EGender, job mosaic.EJob) *Role {
	return &Role{
		RoleInfo: &mosaic.RoleInfo{
			Id:         Instance.NewId(),
			Name:       name,
			Gender:     gender,
			Job:        job,
			Level:      1,
			LogoutTime: time.Unix(0, 0),
		},
		CTime: time.Now(),
	}
}

func (self *Role) Pack() *collection.DbRole {
	return &collection.DbRole{
		Id:         self.Id,
		Name:       self.Name,
		Gender:     int(self.Gender),
		Job:        int(self.Job),
		Level:      self.Level,
		LogoutTime: self.LogoutTime,
		CTime:      self.CTime,
		Account:    self.Account,
	}
}
