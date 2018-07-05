package role

import (
	"app/db"
	"app/db/collections"
	"time"
	"app/common/class"
	//"app"
	"app/network"
	"app"
)

type RoleMgr struct {
	RoleById map[uint64]*Role
	RoleBySesId map[int64]*Role
	SessionByHandle map[string]network.Session
	idCounter uint32
}

func (self *RoleMgr) Init() {
	self.RoleById = make(map[uint64]*Role)
	self.RoleBySesId = make(map[int64]*Role)
}

func (self *RoleMgr) Start() {}
func (self *RoleMgr) Stop() {}

func (self *RoleMgr) NewId() uint64 {
	time := time.Now().Unix() // 时间不回调基本不会冲突
	serverId := uint16(app.Master.ServerId)
	self.idCounter = self.idCounter + 1
	return uint64(time) << 32 | uint64(serverId) << 16 | uint64(self.idCounter)
}

func (self *RoleMgr) CreateRole(name string, gender EGender, job EJob) {
	role := NewRole(self.NewId(), name)
	role.Gender = gender
	role.Job = job
	self.RoleById[role.Id] = role
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

var Instance *RoleMgr
func New() *RoleMgr {
	Instance = &RoleMgr{}
	return Instance
}
