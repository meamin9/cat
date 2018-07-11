package apptime

// 定时器

import (
	"container/heap"
	"time"
)

type Timer struct {
	trigTime  time.Time
	trigEvent func()
	loopTime  time.Duration // 小于等于0时不循环
	index     int
}

func (self *Timer) Stop() {
	if self.index > 0 {
		heap.Remove(Instance, self.index)
	}
}

type TimerHeap []*Timer

func (self TimerHeap) Len() int { return len(self) }
func (self TimerHeap) Less(i, j int) bool {
	return self[i].trigTime.Before(self[j].trigTime)
}
func (self TimerHeap) Swap(i, j int) {
	self[i], self[j] = self[j], self[i]
	self[i].index, self[j].index = j, i
}
func (self *TimerHeap) Push(x interface{}) {
	timer := x.(*Timer)
	list := *self
	timer.index = len(list)
	*self = append(list, timer)
}
func (self *TimerHeap) Pop() interface{} {
	list := *self
	n := len(list)
	timer := list[n-1]
	timer.index = -1
	*self = list[:n-1]
	return timer
}

func (self TimerHeap) Top() *Timer {
	if len(self) > 0 {
		return self[0]
	}
	return nil
}

func (self *TimerHeap) AddTimer(interval time.Duration, cb func(), loop bool) *Timer {
	timer := &Timer{
		trigTime:  Instance.Now().Add(interval),
		trigEvent: cb,
	}
	if loop {
		timer.loopTime = interval
	}
	heap.Push(self, timer)
	return timer
}

func (self *TimerHeap) AddTimerWithDelay(loopTime time.Duration, cb func(), delay time.Duration) *Timer {
	timer := &Timer{
		trigTime:  Instance.Now().Add(delay + loopTime),
		trigEvent: cb,
		loopTime:  loopTime,
	}
	heap.Push(self, timer)
	return timer
}

func (self *TimerHeap) ProcTimer(now time.Time) {
	for m := self.Top(); m != nil; m = self.Top() {
		if now.Before(m.trigTime) {
			break
		}
		if m.loopTime <= 0 {
			heap.Pop(self) // 等价 heap.remove(self, m.index)
		}
		next := m.trigTime.Sub(now) + m.loopTime
		if next <= 0 {
			next = next + m.loopTime*(-next)/m.loopTime + m.loopTime
		}
		m.trigTime = now.Add(next)
		heap.Fix(self, m.index)
		// 需要在回调前处理完堆，因为回调可能会改变堆root节点
		m.trigEvent()
	}
}
