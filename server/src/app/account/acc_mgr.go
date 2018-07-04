package account

import (
	"time"
	"app"
)

// === account ===

type roleinfo struct {
	Id    uint64
	Name  string
	LogoutTime time.Time
	Gender int
	Level int
	Job int
}

type Account struct {
	Id string
	Roles []*roleinfo
}

type AccountMgr struct {
	*Cfg
	AcctById map[string]*Account
}

func (self *AccountMgr) Init() {
	regProp()
}
func (self *AccountMgr) Start() {}
func (self *AccountMgr) Stop() {}


func (self *AccountMgr) AddAccount(acct *Account) {
	self.AcctById[acct.Id] = acct
}

func (self *AccountMgr) unpackAccount(data interface{}) {
	//msg := data.(map[string]interface{})
	//acc := &Account{
	//	Id: msg["_id"].(string),
	//}
	//acc.Roles = msg["roles"].()

}
//
//func (self *account) unpackRoles(datas []map[string]interface{}) {
//	self.roles = make([]*roleinfo, len(datas))
//	for i, d := range datas {
//		self.roles[i] = &roleinfo{
//			d["id"],
//			d["name"],
//			d["level"],
//		}
//	}
//}

//
//
//
//func (self *account) packRoles() interface{} {
//	ids := make([]int64, len(self.roles))
//	for i, r := range self.roles {
//		ids[i] = r.id
//	}
//	return ids
//}
//
//func (self *account) data() interface{} {
//	p := proto.SCRoleList{}
//	roles := make([]*proto.RoleBase, len(self.roles))
//	for i, r := range self.roles {
//		roles[i] = &proto.RoleBase{
//			Id:    r.id,
//			Name:  r.name,
//			Level: r.level,
//		}
//	}
//	ack.Roles = roles
//	return &ack
//}
//
//func newAccount(sid int64, name, pwd string) *account {
//	a := &account{make([]int64, 1), name, pwd, make([]*roleinfo, 0, 1)}
//	a.sids[0] = sid
//	return a
//}
//
//// === accountmgr ===
//
//type mgr struct {
//	accounts map[int64]*account
//}
//
//func (me *mgr) Add(a *account) {
//	me.accounts[a.id] = a
//}
//
//func (self *mgr) CloseAccount(aid, sid int64) {
//	a, ok := self.accounts[aid]
//	if !ok {
//		return
//	}
//	if len(a.sids) > 1 {
//		for i, id := range a.sids {
//			if id == sid {
//				a.sids = append(a.sids[:i], a.sids[i+1:])
//				return
//			}
//		}
//		// error
//		panic("can not find sid")
//	}
//	delete(me.accounts, aid)
//}
//
//func (me *mgr) Account(aid int64) (a *account, ok bool) {
//	a, ok = me.accounts[aid]
//	return
//}


var Instance *AccountMgr
func init() {
	Instance = &AccountMgr{}
	app.Master.RegService(Instance, true)
}
