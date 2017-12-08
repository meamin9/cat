package class

// calss 包括简单的自定义类型，自定义的类型值
// type是关键字，这里用class代替了

type Gender byte

const (
	Female Gender = iota
	Male
	Unmale
)
