package role

import (
	"app/db"
	"app/db/collections"
	"time"
	"app/common/class"
	"app"
)

type RoleMgr struct {
	roles map[int64]Role
	id    int64
}

func NewRoleMgr() *RoleMgr {
	return &RoleMgr{
		roles: make(map[int64]Role),
		id:    10000,
	}
}

func (self *RoleMgr) CreateRole() {

}



// LoadRole 从数据库加载一个role
func (self *mgr) LoadRole(id int64) {
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return collections.RoleLoad(id)
		},
		Result: func(data interface{}, err error) {
			if err == nil {
				role := newRole()
				role.Unpack(data.(map[string]interface{}))
				self.roles[role.Id()] = role
			}
		},
	})
}

func (self *mgr) CreateRole(name string) *Role {
	role := newRole()
	role.id = self.NextId()
	role.birth = time.Now()
	role.name = name
	role.gender = class.Unmale
	return role
}
