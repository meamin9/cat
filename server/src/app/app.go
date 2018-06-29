package app

import "app/service"

var _app = struct {
	serviceMgr *service.ServiceMgr
}{}

func main() {
	_app.serviceMgr = service.NewServiceMgr().(*service.ServiceMgr)
	_app.serviceMgr.Install();
}

func installService() {
	//_app.serviceMgr.InstallService()

}
func Service(serviceName string) service.IService {
	return _app.serviceMgr.Get(serviceName)
}

func ServiceMgr() *service.ServiceMgr {
	return _app.serviceMgr
}
