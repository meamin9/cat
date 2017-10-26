package schedule

import (
	"time"
	"utils"
)

type TimerType byte

const (
	TimerInvalid TimerType = iota
	TimerLoop
	TimerOnce
)

type ITimer interface {
	Stop()
}

type Timer struct {
	cb       func(time.Duration)
	next     time.Time
	interval time.Duration
	class    TimerType
}

func (me *Timer) Stop() {
	me.class = TimerInvalid
}

// timemgr: 1，提供延迟调用和定点调用； 2，服务器时间获取
type timemgr struct {
	now       time.Time
	offset    time.Duration // 服务器时间与系统时间偏差
	monthCbs  map[int]func()
	dayCbs    map[int]func()
	secondCbs map[int]func()
	heap      *utils.MinHeap
	id        int
}

func (me *timemgr) Now() time.Time {
	return me.now
}

func (me *timemgr) Update(systime time.Time) {
	old := me.now
	me.now = systime.Add(me.offset)
	switch {
	case old.Month() != me.now.Month():
		triggerUpdate(me.monthCbs)
		fallthrough
	case old.Day() != me.now.Day():
		triggerUpdate(me.dayCbs)
		fallthrough
	case old.Second() != me.now.Second():
		triggerUpdate(me.secondCbs)
	}
	updateTimer(me.now, me.heap)
}

func triggerUpdate(cbs map[int]func()) {
	// TODO: protected call
	for _, f := range cbs {
		f()
	}
}

func updateTimer(now time.Time, heap *utils.MinHeap) {
	for m := heap.Min(); m != nil; m = heap.Min() {
		t := m.(*Timer)
		if t.class == TimerInvalid {
			heap.EraseMin()
		} else if now.Before(t.next) {
			break
		} else {
			elapsed := now.Sub(t.next) + t.interval
			if t.class == TimerLoop {
				t.next = t.next.Add(t.interval)
				heap.ShiftDown(0)
			} else {
				heap.EraseMin()
			}
			// 需要在回调前处理完堆，因为回调可能会改变堆root节点
			t.cb(elapsed)
		}
	}
}

// interval单位是纳秒，值必须大于0（会强制最小值为1）
func (me *timemgr) StartTimer(interval time.Duration, cb func(time.Duration), loop bool) ITimer {
	if interval <= 0 {
		interval = 1
	}
	class := TimerOnce
	if loop {
		class = TimerLoop
	}
	t := &Timer{
		cb:       cb,
		class:    class,
		interval: interval,
		next:     me.Now().Add(interval),
	}
	me.heap.Insert(t)
	return t
}

func NewTimemgr() *timemgr {
	compareTimer := func(a, b interface{}) bool {
		return a.(*Timer).next.Before(b.(*Timer).next)
	}
	return &timemgr{
		now:    time.Now(),
		offset: 0,
		heap:   utils.NewMinHeap(compareTimer, 100),
	}
}
