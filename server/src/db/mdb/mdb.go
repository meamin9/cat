package mdb

import (
	"github.com/davyxu/golog"
	"gopkg.in/mgo.v2"
	"sync"
	"time"
)

const dburl = "cat:123456@localhost:27017/cat"

var log *golog.Logger = golog.New("mdb")

var mdb = struct {
	session *mgo.Session
	m       *sync.Mutex
	w       *sync.WaitGroup
}{}

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

func GetSession() *mgo.Session {
	if mdb.session == nil {
		connect()
	}
	return mdb.session.Clone()
}

func Send(run func()) {
	mdb.w.Add(1)
	go func() {
		run()
		mdb.w.Done()
	}()
}

func Finish() {
	mdb.w.Wait()
}

func init() {
	mdb.m = new(sync.Mutex)
	mdb.w = new(sync.WaitGroup)
}
