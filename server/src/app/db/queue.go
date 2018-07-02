package db
//
//import (
//	"errors"
//	"app"
//)
//
//type ISql interface {
//	Exec() (interface{}, error)
//}
//
//type Mail struct {
//	Sql ISql
//	Cb func(interface{}, error)
//}
//
//type dbqueue struct {
//	sendQue chan *Mail
//	recvQue chan func()
//	exit chan bool
//}
//
//func newDbqueue(size int) *dbqueue {
//	return &dbqueue{
//		sendQue: make(chan *Mail, size),
//		recvQue: make(chan func(), size),
//	}
//}
//
//func (self *dbqueue) Add(mail *Mail) {
//	select {
//	case self.sendQue <- mail:
//	default:
//		// log.Errorln("数据库发送队列已满")
//		// 直接go一个，避免阻塞主线程
//		go func() {
//			self.sendQue <- mail
//		}()
//	}
//}
//
//func (self *queue) Stop() {
//	self.Add(nil)
//}
//
//func (self *queue) waitRequest() {
//	for {
//		evt := <-self.evts
//		ret, err := evt.Sql.Exec()
//		if err == ErrExit {
//			self.exitSignal <- true
//			break
//		} else if err != nil {
//			self.Log.Errorln(err)
//		}
//		if evt.Cb != nil {
//			q.responses <- func() {
//				req.Result(retdata, err)
//			}
//		}
//	}
//}
//
//func (q *queue) Start() {
//	go func() {
//		q.waitRequest()
//	}()
//}
//
//func (q *queue) Stop() {
//	q.Send(&Request{
//		func() (interface{}, error) {
//			return nil, ErrExit
//		},
//		nil,
//	})
//	<-q.exitSignal
//}
//
//func (q *queue) Poll() {
//Loop:
//	for {
//		select {
//		case res := <-q.responses:
//			res()
//		default:
//			break Loop
//		}
//	}
//}
//
//func (q *queue) C() chan Response {
//	return q.responses
//}
//
//func NewQueue() *queue {
//	return &queue{
//		make(chan *Request, 100),
//		make(chan Response, 100),
//		make(chan bool, 0),
//	}
//}
