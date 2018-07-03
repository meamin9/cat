package apptime

import (
	"time"
	"app"
)

type TimeMgr struct {
	offset time.Duration
	now time.Time
	tick *time.Ticker
}

func (self *TimeMgr) Init() {
	self.offset = 0
	self.now = time.Now()
}

func (self *TimeMgr) Start() {
	self.tick = time.NewTicker(1)
}

func (self *TimeMgr) Stop() {
	self.tick.Stop()
}

func (self *TimeMgr) C() <-chan time.Time {
	return self.tick.C
}

func (self *TimeMgr) Now() time.Time {
	return self.now
}

func (self *TimeMgr) Tick(t time.Time) {
	self.now = t.Add(self.offset)
}

var Instance *TimeMgr
func init() {
	Instance = &TimeMgr{}
	app.Master.RegService(Instance, true)
}
