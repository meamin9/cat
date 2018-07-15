package account

/* account
登录、角色创建管理
一个用户可以创建多个角色
*/

import (
	"app/mosaic"
)

type Account struct {
	Id    string
	Roles []*mosaic.RoleInfo
}

func (self *Account) AddRoleInfo(info *mosaic.RoleInfo) {
	self.Roles = append(self.Roles, info)
}

type AccountMgr struct {
	acctById  map[string]*Account
	acctBySid map[int64]*Account
}

func (self *AccountMgr) Init() {
	regProp()
}

func (self *AccountMgr) AddLoginAccount(acct *Account, sid int64) {
	self.acctById[acct.Id] = acct
	self.acctBySid[sid] = acct
}

func (self *AccountMgr) AccountById(id string) *Account {
	return self.acctById[id]
}

// find account by session id
func (self *AccountMgr) AccountBySid(sid int64) *Account {
	return self.acctBySid[sid]
}

var Instance *AccountMgr
func New() *AccountMgr {
	Instance = &AccountMgr{}
	return Instance
}
