package collection

type CollectionMgr struct {}

func (self *CollectionMgr) Init() {
	initRole()
}

func New() *CollectionMgr {
	return &CollectionMgr{}
}
