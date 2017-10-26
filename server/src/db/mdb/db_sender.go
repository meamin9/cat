package mdb

import (
	"gopkg.in/mgo.v2"
)

/*
排队的db发送接受线程
*/

type RetCode int

const (
	Success RetCode = iota
	Failed
	AlreadyExist
	NotFound
)

type Request struct {
	query  func(db *mgo.Database) (retdata []interface{}, retcode RetCode)
	result func([]interface{}, RetCode)
}

type Response func()

type Db struct {
	requests  chan *Request
	responses chan *Response
}

func (db *Db) AddRequest(req *Request) {
	select {
	case db.requests <- req:
	default:
		log.Errorln("数据库发送队列已满，请求被丢失")
	}
}

func (db *Db) waitRequest() {
	for {
		req := <-db.requests

		retdata, retcode := req.query(GetDb())
		var response Response = func() {
			req.result(retdata, retcode)
		}
		db.responses <- &response
	}
}

func (db *Db) Start() {
	go func() {
		db.waitRequest()
	}()
}

func (db *Db) Poll() {
Loop:
	for {
		select {
		case res := <-db.responses:
			(*res)()
		default:
			break Loop
		}
	}
}

func NewDb() *Db {
	return &Db{}
}
