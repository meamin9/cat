package db

import (
	"github.com/davyxu/golog"
	"gopkg.in/mgo.v2"
	"sync"
	"time"
)

const dburl = "cat:123456@localhost:27017/cat"

var dbname string = "cat"

var log *golog.Logger = golog.New("db")

var mdb = struct {
	session *mgo.Session
	m       *sync.Mutex
	w       *sync.WaitGroup
	q       *queue
}{}

// 测试用函数
func SetDbname(name string) {
	dbname = name
}

var exitBegin chan bool
var exitEnd chan bool

func InstallTest() {
	SetDbname("cat_test")
	exitBegin = make(chan bool)
	exitEnd = make(chan bool)

	Queue().Start()
	go func() {
		for {
			t0 := time.Now()
			Queue().Poll()
			t1 := time.Now()
			elapsed := t1.Sub(t0)
			f := time.Second / 60
			select {
			case <-exitBegin:
				exitEnd <- true
			default:
				if elapsed < f {
					time.Sleep(f - elapsed)
				}
			}
		}
	}()
}

func StopTest() {
	time.Sleep(time.Second)
	Queue().Stop()
	exitBegin <- true
	<-exitEnd
}

// ===== 测试函数结束 =====

// 连接db，如果连接失败会阻塞，并每秒尝试重连，直到连接成功
func connect() {
	mdb.m.Lock()
	defer mdb.m.Unlock()
	if mdb.session != nil {
		return
	}
	session, err := mgo.Dial(dburl)
	for err != nil {
		log.Errorln("connect mdb failed", dburl)
		time.Sleep(time.Second)
		session, err = mgo.Dial(dburl)
	}
	mdb.session = session
}

func getSession() *mgo.Session {
	if mdb.session == nil {
		connect()
	}
	return mdb.session.Clone()
}

func DB() *mgo.Database {
	return getRawSession().DB(dbname)
}

func getRawSession() *mgo.Session {
	if mdb.session == nil {
		connect()
	}
	return mdb.session
}

func send(run func()) {
	mdb.w.Add(1)
	go func() {
		run()
		mdb.w.Done()
	}()
}

func finish() {
	mdb.w.Wait()
}

func Queue() *queue {
	return mdb.q
}

func init() {
	mdb.m = new(sync.Mutex)
	mdb.w = new(sync.WaitGroup)
	mdb.q = NewQueue()
}
