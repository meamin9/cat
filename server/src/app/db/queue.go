package db

import (
	"errors"
)

var ErrExit = errors.New("exit")

type Event struct {

}

type Request struct {
	Quest  func() (retdata interface{}, err error)
	Result func(interface{}, error)
}

func NewRequest(quest func() (interface{}, error), result func(interface{}, error)) *Request {
	return &Request{quest, result}
}

type Response func()

type queue struct {
	requests   chan *Request
	responses  chan Response
	exitSignal chan bool
}

func (q *queue) Send(req *Request) {
	select {
	case q.requests <- req:
	default:
		log.Errorln("数据库发送队列已满")
		// 直接go一个，避免阻塞主线程
		go func() {
			q.requests <- req
		}()
	}
}

func (q *queue) Push(quest func() (interface{}, error), result func(interface{}, error)) {
	q.Send(&Request{quest, result})
}

func (q *queue) waitRequest() {
	for {
		req := <-q.requests
		retdata, err := req.Quest()
		if err != nil {
			log.Errorln(err)
		}
		if err == ErrExit {
			q.exitSignal <- true
			break
		}
		if req.Result != nil {
			q.responses <- func() {
				req.Result(retdata, err)
			}
		}
	}
}

func (q *queue) Start() {
	go func() {
		q.waitRequest()
	}()
}

func (q *queue) Stop() {
	q.Send(&Request{
		func() (interface{}, error) {
			return nil, ErrExit
		},
		nil,
	})
	<-q.exitSignal
}

func (q *queue) Poll() {
Loop:
	for {
		select {
		case res := <-q.responses:
			res()
		default:
			break Loop
		}
	}
}

func (q *queue) C() chan Response {
	return q.responses
}

func NewQueue() *queue {
	return &queue{
		make(chan *Request, 100),
		make(chan Response, 100),
		make(chan bool, 0),
	}
}
