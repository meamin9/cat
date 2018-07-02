package role

import "app"

type RoleSvc struct {
	app.ServiceBase
	RoleMgr
}

func (self *RoleSvc) Install() {
	app.NetSvc.RegProto()

}
