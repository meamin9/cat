package app

import "service"

var _app = struct {
	serviceMgr *service.ServiceMgr
}{}

func Run() {
	_app.serviceMgr = service.NewServiceMgr().(*service.ServiceMgr)
	_app.serviceMgr.Install();
}

func ServiceMgr() *service.ServiceMgr {
	return _app.serviceMgr
}
