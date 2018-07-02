package utils

// 使用chan的队列

type Cqueue struct {
	que chan interface{}
	exit chan bool
}

func NewCqueue(size int) *Cqueue{
	return &Cqueue{
		que: make(chan interface{}, size),
		exit: make(chan bool),
	}
}
