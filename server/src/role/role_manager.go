package role

import (
	"base"
	//"cellnet"
)

var mgr = struct {
	roles    map[base.RoleId]*Role
	sessions map[int64]*Role
}{
	make(map[base.RoleId]*Role, 100),
	make(map[int64]*Role, 100),
}

func init() {

}

func GetRoleBySid(sid int64) (role *Role, ok bool) {
	role, ok = mgr.sessions[sid]
	return
}

func GetRole(id base.RoleId) (role *Role, ok bool) {
	role, ok = mgr.roles[id]
	return
}
