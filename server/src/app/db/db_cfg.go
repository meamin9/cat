package db

type DbCfg struct {
	Username   string
	Password    string
	Addrs   string
	Dbname string
	url    string
}

func (self *DbCfg) LoadCfg() {
	//self.user = "cat"
	//self.addr = "localcast"
	//self.pwd = "123456"
	//self.dbname = "cat"
	// 格式 mongodb://username:password@addr1:port1,...,addrN:portN/dbname?key1=value1&key2=value2
	//self.url = fmt.Sprintf("%v:%v@%v:%v/%v", self.user, self.pwd, self.addr, self.port, self.dbname)
}

func (self *DbCfg) Url() string {
	return self.url
}
