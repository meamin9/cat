package db

import "fmt"

type DbCfg struct {
	user   string
	pwd    string
	dbname string
	addr   string
	port   string
	url    string
}

func (self *DbCfg) LoadCfg() {
	self.user = "cat"
	self.addr = "localcast"
	self.pwd = "123456"
	self.dbname = "cat"
	self.url = fmt.Sprintf("%v:%v@%v:%v/%v", self.user, self.pwd, self.addr, self.port, self.dbname)
}

func (self *DbCfg) Url() string {
	return self.url
}
