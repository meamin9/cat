package db

import (
	"app"
	"gopkg.in/mgo.v2"
	"sync"
)

type ISql interface {
	Exec() (interface{}, error)
}

type Mail struct {
	Sql ISql
	Cb func(interface{}, error)
}

type DbSvc struct {
	app.ServiceBase

	DbCfg
	session *mgo.Session
	queue   chan *Mail
	cbqueue chan func()
	waitExit sync.WaitGroup
}

func NewDbSrv() *DbSvc {
	return &DbSvc{}
}

func (self *DbSvc) Install() {
	self.LoadCfg()
	var err error
	self.session, err = mgo.Dial(self.url)
	if err != nil { // 数据库
		self.Log.Errorf("db connet failed %v", err)
	}
	self.queue = make(chan *Mail, 64)
	self.cbqueue = make(chan func(), 64)
	self.SetStatus(app.Working)
}

func (self *DbSvc) Send(mail *Mail) {
	select {
	case self.queue <- mail:
	default:
		self.Log.Errorln("db queue is full")
		go func() {
			self.queue <- mail
		}()
	}
}

func (self *DbSvc) Run() {
	go self.ProcMail()
}

// 阻塞直到db操作完成，db返回队列依然可用
func (self *DbSvc) Stop() {
	self.waitExit.Add(1)
	close(self.queue)
	self.waitExit.Wait()
}

func (self *DbSvc) ProcMail() {
	for {
		if mail, ok := <- self.queue; ok {
			//TODO: panic恢复
			ret, err := mail.Sql.Exec()
			if err != nil {
				self.Log.Errorf("sql error %v", err)
			}
			if mail.Cb != nil {
				self.cbqueue <- func() {
					mail.Cb(ret, err)
				}
			}
		} else {
			self.waitExit.Done()
			break
		}
	}
}

func (self *DbSvc) ProcMailResponse() {
	for {
		select {
		case cb := <- self.cbqueue: // 这里会频繁的加锁解锁，考虑不用chan队列做
			cb()
		default:
		}
	}
}

func (self *DbSvc) Session() *mgo.Session {
	if self.session == nil {
		var err error
		self.session, err = mgo.Dial(self.url)
		if err != nil {
			panic("db connet failed")
		}
	}
	return self.session.Clone()
}

// 返回一个db的新会话
func (self *DbSvc) DB() *mgo.Database {
	return self.Session().DB(self.dbname)
}

var svc * DbSvc
func init() {
	svc = &DbSvc{}
	app.Master.RegService(svc, "db", app.PriorBase)
}
