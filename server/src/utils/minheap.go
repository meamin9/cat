package utils

type IMinhead interface {
	Insert(e interface{})
	Index(e interface{})
	Erase(i int)
	SetLt(func(a interface{}, b interface{}) bool)
}
type Minhead struct {
	arr  []interface{}
	ltop func(a interface{}, b interface{}) bool
}

func (self *Minhead) shiftUp(i int) {
	for i > 0 {
		j := (i - 1) / 2
		if self.ltop(self.arr[j], self.arr[i]) {
			self.arr[i], self.arr[j] = self.arr[j], self.arr[i]
			i = j
		} else {
			break
		}
	}
}

func (self *Minhead) Insert(e interface{}) {

}
