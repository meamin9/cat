package db

import (
	"gopkg.in/mgo.v2"
	"time"
)

const dburl = "cat:123456@localhost:27017/cat"

var mdb = struct {
	session *mgo.Session

}{}

func RunMdb() {
	for mdb.session == nil {
		var err error
		session, err := mgo.Dial(dburl); err != nil {
			log.Errorln(err)
			time.Sleep(5 * time.Second)
		}
		mdb.session = session
		defer mdb.session.Close()
		break
	}

}
func NewMdb() {

}
