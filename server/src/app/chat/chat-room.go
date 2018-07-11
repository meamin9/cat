package chat

import (
	"app/role"
	"base"
)

type iroom interface {
	visitMember(func(*role.Role))
}

type baseRoom struct {
	msgs     []*chatMsg
	msgBegin int
}

type privateRoom struct {
	baseRoom
	members []base.RoleId
}

func (self *privateRoom) visitMember(handle func(*role.Role)) {
	for _, id := range self.members {
		if r, ok := role.GetRole(id); ok {
			handle(r)
		}
	}
}

type worldRoom struct {
	baseRoom
}

func (self *worldRoom) visitMember(handle func(*role.Role)) {

}
