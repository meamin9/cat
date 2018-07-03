package account

import (
	"time"
	"app/network"
)

// === account ===

type roleinfo struct {
	Id    uint64
	Name  string
	LogoutTime time.Time
	Gender int
	Level int
}

type Account struct {
	Id string
	Roles []*roleinfo
}

type AccountMgr struct {
	AcctBySesId map[int64]network.Session
}


func (self *account) unpackRoles(datas []map[string]interface{}) {
	self.roles = make([]*roleinfo, len(datas))
	for i, d := range datas {
		self.roles[i] = &roleinfo{
			d["id"],
			d["name"],
			d["level"],
		}
	}
}




func (self *account) packRoles() interface{} {
	ids := make([]int64, len(self.roles))
	for i, r := range self.roles {
		ids[i] = r.id
	}
	return ids
}

func (self *account) data() interface{} {
	p := proto.SCRoleList{}
	roles := make([]*proto.RoleBase, len(self.roles))
	for i, r := range self.roles {
		roles[i] = &proto.RoleBase{
			Id:    r.id,
			Name:  r.name,
			Level: r.level,
		}
	}
	ack.Roles = roles
	return &ack
}

func newAccount(sid int64, name, pwd string) *account {
	a := &account{make([]int64, 1), name, pwd, make([]*roleinfo, 0, 1)}
	a.sids[0] = sid
	return a
}

// === accountmgr ===

type mgr struct {
	accounts map[int64]*account
}

func (me *mgr) Add(a *account) {
	me.accounts[a.id] = a
}

func (self *mgr) CloseAccount(aid, sid int64) {
	a, ok := self.accounts[aid]
	if !ok {
		return
	}
	if len(a.sids) > 1 {
		for i, id := range a.sids {
			if id == sid {
				a.sids = append(a.sids[:i], a.sids[i+1:])
				return
			}
		}
		// error
		panic("can not find sid")
	}
	delete(me.accounts, aid)
}

func (me *mgr) Account(aid int64) (a *account, ok bool) {
	a, ok = me.accounts[aid]
	return
}

var _instance *mgr

func Mgr() *mgr {
	return _instance
}

func init() {
	_instance = &mgr{
		accounts: make(map[int64]*account),
	}
}
