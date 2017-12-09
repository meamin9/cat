package account

//正在登录的还没有进入游戏的帐号
// 单例

type account struct {
	name  string
	roles []int64
}

func (me *account) Add(rid int64) {
	me.roles = append(me.roles, rid)
}

func NewAccount(name string, roles []int64) *account {
	return &account{name, roles}
}

// === accountmgr ===

type mgr struct {
	accounts map[int64]*account
}

func newmgr() *mgr {
	return &mgr{
		accounts: make(map[int64]*account),
	}
}

var _instance *mgr

func init() {
	_instance = newmgr()
}

func AccountMgr() *mgr {
	return _instance
}

func (me *mgr) Add(sid int64, a *account) {
	me.accounts[sid] = a
}

func (me *mgr) Remove(sid int64) {
	delete(me.accounts, sid)
}

func (me *mgr) Get(sid int64) (a *account, ok bool) {
	a, ok = me.accounts[sid]
	return
}
