package account

type Cfg struct {
	NameLenRange [2]int
	PwdLenRange [2]int
}

func (self *Cfg) LoadCfg() {
	self.NameLenRange = [2]int{6, 32}
	self.PwdLenRange = [2]int{6, 32}
}
