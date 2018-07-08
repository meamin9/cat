package app

import (
	"os"
	"path/filepath"
	"encoding/json"
)

type AppCfg struct {
	ServerName string
	ServerId uint16
	ServerAddr string
	Db struct{
		Username   string
		Password    string
		Addrs   string
		Dbname string
	}
	Cfgpath string // 配置目录
}

func (self *AppCfg) initPath() {
	p, err := os.Executable()
	if err != nil {
		panic("path read error")
	}
	self.Cfgpath = filepath.Join(filepath.Dir(p), "appdata")
}

func (self *AppCfg) LoadCfg() {
	self.initPath()
	p := filepath.Join(self.Cfgpath, "appcfg.json")
	f, err := os.Open(p)
	if err != nil {
		panic(err)
	}
	defer f.Close()
	//var c
	json.Unmarshal(f.Read(), self)


}
