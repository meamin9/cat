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

var Instance *AccountMgr
func New() *AccountMgr{
	Instance = &AccountMgr{}
	return Instance
}
