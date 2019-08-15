package apptime

import (
	"app/db"
	"math/rand"
	"time"
)

type IDbPacker interface {
	Pack() db.ISql
}

// 同步定时打包存盘
type TimeSaveHelper struct {
	master IDbPacker
	dirty  bool
	timer  *Timer
}

func (self *TimeSaveHelper) Start() {
	loopTime := time.Minute * 5
	delay := time.Duration(rand.Intn(int(loopTime)))
	self.timer = Instance.AddTimerWithDelay(loopTime, self.Stop, delay)
}

func (self *TimeSaveHelper) Stop() {
	self.timer.Stop()
	self.timer = nil
}

func (self *TimeSaveHelper) Save() {
	if self.dirty {
		sql := self.master.Pack()
		db.Instance.Send(&db.dbEvent{
			Sql: sql,
		})
	}
}

func NewTimeSaveHelper(master IDbPacker) *TimeSaveHelper {
	return &TimeSaveHelper{
		master: master,
	}
}
