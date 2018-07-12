package appinfo

import (
	"app/util"
	"fmt"
	"os"
	"path/filepath"
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
	Dburl string
	Dbname string
	Datapath string
	ServerDatapath string //服务器自身配置的路径
)

func init() {
	argsCount := len(os.Args)
	ServerDatapath = "appdata"
	Datapath = "data"
	if argsCount > 1 {
		ServerDatapath = os.Args[1]
	}
	if argsCount > 2 {
		Datapath = os.Args[2]
	}
	res := appjson{}
	path := filepath.Join(ServerDatapath, "appcfg.json")
	util.LoadJson(path, &res)
	ServerId = res.ServerId
	ServerName = res.ServerName
	// db & net
	dbcfg := res.Db
	Dburl = fmt.Sprintf("%v:%v@%v/%v", dbcfg.Username, dbcfg.Password, dbcfg.Addrs, dbcfg.Dbname)
	Dbname = dbcfg.Dbname
}
