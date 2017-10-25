package schedule

import (
	"time"
)

type timer struct {
	cb       func(time.Duration)
	next     time.Time
	interval time.Duration
}

type timemgr struct {
	now          time.Time
	offset       time.Duration // 服务器时间与系统时间偏差
	monthCbs     map[int]func()
	dayCbs       map[int]func()
	secondCbs    map[int]func()
	looptimers   map[int]timer
	onceTimes    map[int]timer
	isTriggering bool
	id           int
}

func (self *timemgr) Now() time.Time {
	return self.now
}

func (self *timemgr) Update(systime time.Time) {
	old := self.now
	self.now = systime.Add(self.offset)
	switch {
	case old.Month() != self.now.Month():
		triggerUpdate(self.monthCbs)
		fallthrough
	case old.Day() != self.now.Day():
		triggerUpdate(self.dayCbs)
		fallthrough
	case old.Second() != self.now.Second():
		triggerUpdate(self.secondCbs)
	}
	updateTimer(self.now, self.looptimers)
	updateTimer(self.now, self.onceTimes)
}

func triggerUpdate(cbs map[int]func()) {
	// TODO: protected call
	for _, f := range cbs {
		f()
	}
}

func updateTimer(now time.Time, timers map[int]timer) {
	for _, t := range timers {
		if !now.Before(t.next) {
			elapsed := now.Sub(t.next) + t.interval
			t.next = t.next.Add(t.interval)
			t.cb(elapsed)
		}
	}
}
