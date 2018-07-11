package apptime

import (
	"app/util"
	"time"
)

type TimeMgr struct {
	offset time.Duration
	now    time.Time
	tick   *time.Ticker

	TimerHeap
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

func (self *TimeMgr) Chan() <-chan time.Time {
	return self.tick.C
}

func (self *TimeMgr) Now() time.Time {
	return self.now
}

func (self *TimeMgr) Tick(t time.Time) {
	self.now = t.Add(self.offset)
	self.ProcTimer(self.now)
}

var Instance *TimeMgr
var log *util.Logger

func New() *TimeMgr {
	Instance = &TimeMgr{}
	log = util.NewLog("apptime")
	return Instance
}
