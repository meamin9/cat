package role

type mgr struct {
	roles map[int64]Role
	id    int64
}

func newmgr() *mgr {
	return &mgr{
		roles: make(map[int64]Role),
		id:    10000,
	}
}

var _instance *mgr

func init() {
	_instance = newmgr()
}

func RoleMgr() *mgr {
	return _instance
}

func (me *mgr) NextId() int64 {
	me.id += 1
	return me.id
}
