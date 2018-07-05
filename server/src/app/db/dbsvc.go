package db

import (
	"app"
	"gopkg.in/mgo.v2"
	"sync"
)

type ISql interface {
	Exec(ses *mgo.Session) (interface{}, error)
}

type Mail struct {
	Sql ISql
	Cb func(interface{}, error)
}


type DbSvc struct {
	app.ServiceBase

	*DbCfg
	session *mgo.Session
	queue   chan *Mail
	cbqueue chan func()
	exitSync sync.WaitGroup
}

func (self *DbSvc) Init() {
	var err error
	self.session, err = mgo.Dial(self.url)
	if err != nil { // 数据库
		self.Log.Errorf("db connet failed %v", err)
	}
	self.queue = make(chan *Mail, 64)
	self.cbqueue = make(chan func(), 64)
}

func (self *DbSvc) Start() {
	self.exitSync.Add(1)
	go func() {
		for {
			if mail, ok := <- self.queue; ok {
				//TODO: panic恢复
				ses := self.Session()
				ret, err := mail.Sql.Exec(ses)
				ses.Close()
				if err != nil {
					self.Log.Errorf("sql error %v", err)
				}
				if mail.Cb != nil {
					self.cbqueue <- func() {
						mail.Cb(ret, err)
					}
				}
			} else {
				self.exitSync.Done()
				break
			}
		}
	}()
}

// 阻塞直到db操作完成，db返回队列依然可用,但此时应该没有pull了
func (self *DbSvc) Stop() {
	close(self.queue)
	self.exitSync.Wait()
	if self.session != nil {
		self.session.Close()
		self.session = nil
	}
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

func (self *DbSvc) Chan() <- chan func() {
	return self.cbqueue
}

func (self *DbSvc) Pull() {
	for {
		select {
		case cb := <- self.cbqueue: // 这里会频繁的加锁解锁，考虑不用chan队列做
			cb()
		default:
		}
	}
}

func (self *DbSvc) RawSession() *mgo.Session {
	if self.session == nil {
		var err error
		self.session, err = mgo.Dial(self.url)
		if err != nil {
			panic("db connet failed")
		}
	}
	return self.session
}

func (self *DbSvc) Session() *mgo.Session {
	return self.RawSession().Clone()
}

func (self *DbSvc) DBName() string {
	return self.dbname
}

func (self *DbSvc) C(name string, ses *mgo.Session) *mgo.Collection {
	if ses == nil {
		ses = self.RawSession()
	}
	return ses.DB(self.dbname).C(name)
}

var Instance *DbSvc
func init() {
	Instance = &DbSvc{}
	app.Master.RegService(Instance, false)
}
