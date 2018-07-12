package db

import (
	"app/util"
	"fmt"
	"gopkg.in/mgo.v2"
	"sync"
	"app/appinfo"
)

type ISql interface {
	Exec(ses *mgo.Session) (interface{}, error)
}

type Mail struct {
	Sql ISql
	Cb  func(interface{}, error)
}

type DbMgr struct {
	session  *mgo.Session
	queue    chan *Mail
	cbqueue  chan func()
	exitSync sync.WaitGroup

	// 配置
	url    string
	Dbname string
}

func (self *DbMgr) SetCfg(username, pwd, addrs, dbname string) {
	// 格式 mongodb://username:password@addr1:port1,...,addrN:portN/dbname?key1=value1&key2=value2
	self.url = fmt.Sprintf("%v:%v@%v/%v", username, pwd, addrs, dbname)
	self.Dbname = dbname
}

func (self *DbMgr) Init() {
	self.queue = make(chan *Mail, 64)
	self.cbqueue = make(chan func(), 64)
	self.Dbname = appinfo.Dbname
	self.url = appinfo.Dburl
	if self.RawSession() == nil {
		panic("connect to db failed")
	}

}

func (self *DbMgr) Start() {
	self.exitSync.Add(1)
	go func() {
		for {
			if mail, ok := <-self.queue; ok {
				//TODO: panic恢复
				ses := self.Session()
				ret, err := mail.Sql.Exec(ses)
				ses.Close()
				if err != nil {
					log.Errorf("sql error %v", err)
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
func (self *DbMgr) Stop() {
	close(self.queue)
	self.exitSync.Wait()
	if self.session != nil {
		self.session.Close()
		self.session = nil
	}
}

func (self *DbMgr) Send(mail *Mail) {
	select {
	case self.queue <- mail:
	default:
		log.Errorln("db queue is full")
		go func() {
			self.queue <- mail
		}()
	}
}

func (self *DbMgr) Chan() <-chan func() {
	return self.cbqueue
}

func (self *DbMgr) Pull() {
	for {
		select {
		case cb := <-self.cbqueue: // 这里会频繁的加锁解锁，考虑不用chan队列做
			cb()
		default:
		}
	}
}

func (self *DbMgr) RawSession() *mgo.Session {
	if self.session == nil {
		var err error
		self.session, err = mgo.Dial(self.url)
		if err != nil {
			log.Errorf("db connet failed: {}", err)
		}
	}
	return self.session
}

func (self *DbMgr) Session() *mgo.Session {
	return self.RawSession().Clone()
}

func (self *DbMgr) C(name string, ses *mgo.Session) *mgo.Collection {
	if ses == nil {
		ses = self.RawSession()
	}
	return ses.DB(self.Dbname).C(name)
}

var Instance *DbMgr
var log *util.Logger

func New() *DbMgr {
	Instance = &DbMgr{}
	log = util.NewLog("db")
	return Instance
}
