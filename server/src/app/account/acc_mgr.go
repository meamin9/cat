package account

/* account
登录、角色创建管理
一个用户可以创建多个角色
*/

import (
	"app/role"
)

type Account struct {
	Id string
	Roles []*role.RoleInfo
}

func (self *Account) AddRoleInfo(info *role.RoleInfo) {
	self.Roles = append(self.Roles, info)
}

type AccountMgr struct {
	*Cfg
	AcctById map[string]*Account
	acctBySid map[int64]*Account
}

func (self *AccountMgr) Init() {
	regProp()
}
func (self *AccountMgr) Start() {}
func (self *AccountMgr) Stop() {}


func (self *AccountMgr) AddAccount(acct *Account) {
	self.AcctById[acct.Id] = acct
}

func (self *AccountMgr) AccountById (id string) *Account {
	return self.AcctById[id]
}

// find account by session id
func (self *AccountMgr) AccountBySid (sid int64) *Account {
	return self.acctBySid[sid]
}

var Instance *AccountMgr
func New() *AccountMgr{
	Instance = &AccountMgr{}
	return Instance
}
