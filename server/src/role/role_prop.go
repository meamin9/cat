package role

import (
	"common/class"
)

const (
	name   int = 5000
	level  int = 2000
	gender int = 6000
	job    int = 7000
)

type RoleProp struct {
	propInt16      [1]int16
	propString     [1]string
	propGendertype [1]class.Gender
	propJobtype    [1]int
}

func (self *RoleProp) GetInt16(key int) int16 {
	return self.propInt16[key%1000]
}

func (self *RoleProp) SetInt16(key int, v int16) {
	self.propInt16[key%1000] = v
}

func (self *RoleProp) GetString(key int) string {
	return self.propString[key%1000]
}

func (self *RoleProp) SetString(key int, v string) {
	self.propString[key%1000] = v
}

func (self *RoleProp) GetGendertype(key int) class.Gender {
	return self.propGendertype[key%1000]
}

func (self *RoleProp) SetGendertype(key int, v class.Gender) {
	self.propGendertype[key%1000] = v
}

func (self *RoleProp) GetJobtype(key int) int {
	return self.propJobtype[key%1000]
}

func (self *RoleProp) SetJobtype(key int, v int) {
	self.propJobtype[key%1000] = v
}

func NewRoleProp() *RoleProp {
	return &RoleProp{}
}
