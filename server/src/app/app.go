package app

import "app/network"

var _app = struct {
	sermgr *ServiceMgr
	netsrc *network.NetSvc
}{}

func main() {
	_app.sermgr = NewServiceMgr().(*ServiceMgr)
	_app.sermgr.Install()
}

func run() {
	_app.sermgr.RunAllService()
	for {
		select {

		}
	}
}

func installService() {
	//_app.serviceMgr.InstallService()

}
func Service(serviceName string) IService {
	return _app.sermgr.Get(serviceName)
}
//var NetSvc = network.NewNetSvc()

func NetSvc() *network.NetSvc {
	return _app.netsrc
}