package account

//正在登录的还没有进入游戏的帐号
// 单例

type roleinfo struct {
	id    int64
	name  string
	level int16
}

type account struct {
	sid   int64
	name  string
	pwd   string
	roles []*roleinfo
}

func (me *account) Add(rid int64) {
	me.roles = append(me.roles, rid)
}

func (self *account) unpackRoleInfo(datas []map[string]interface{}) {
	self.roles = make([]*roleinfo, len(datas))
	for i, d := range datas {
		self.roles[i] = &roleinfo{
			d["id"],
			d["name"],
			d["level"],
		}
	}
}

func NewAccount(sid int64, name, pwd string) *account {
	return &account{sid, name, pwd, make([]*roleinfo, 0)}
}

func NewAccountFromPack(name, pwd string, roles map[string]interface{}) {

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

func (me *mgr) Add(a *account) {
	me.accounts[a.id] = a
}

func (me *mgr) Remove(id int64) {
	delete(me.accounts, id)
}

func (me *mgr) Account(id int64) (a *account, ok bool) {
	a, ok = me.accounts[sid]
	return
}
