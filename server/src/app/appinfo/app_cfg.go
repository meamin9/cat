package appinfo

import (
	"app/db"
	"app/network"
	"app/util"
)

// json 定义
type appjson struct {
	ServerName string
	ServerId   uint16
	ServerAddr string
	Db         struct {
		Username string
		Password string
		Addrs    string
		Dbname   string
	}
}

var (
	ServerName string
	ServerId   uint16
	ServerAddr string
)

func init() {
	res := appjson{}
	util.LoadJson("appcfg.json", &res)
	ServerId = res.ServerId
	ServerName = res.ServerName
	// net & db
	network.Instance.Addr = res.ServerAddr
	dbcfg := res.Db
	db.Instance.SetCfg(dbcfg.Username, dbcfg.Password, dbcfg.Addrs, dbcfg.Dbname)
}
