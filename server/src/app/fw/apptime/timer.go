package apptime

// 定时器
import (
	"container/heap"
	"time"
)

type Timer interface {
	Cancel()
}

type timer struct {
	trigTime  time.Time
	trigEvent func()
	loopTime  time.Duration // 小于等于0时不循环
	index     int
}

func (t *timer) Cancel() {
	if t.index > -1 {
		heap.Remove(Manager, t.index)
		t.index = -1
	}
}

type timerHeap []*timer

func (t timerHeap) Len() int {
	return len(t)
}

func (t timerHeap) Less(i, j int) bool {
	return t[i].trigTime.Before(t[j].trigTime)
}

func (t timerHeap) Swap(i, j int) {
	t[i], t[j] = t[j], t[i]
	t[i].index, t[j].index = j, i
}

func (t *timerHeap) Push(x interface{}) {
	timer := x.(*timer)
	list := *t
	timer.index = len(list)
	*t = append(list, timer)
}

func (t *timerHeap) Pop() interface{} {
	list := *t
	n := len(list)
	timer := list[n-1]
	*t = list[:n-1]
	return timer
}

func (t timerHeap) Top() *timer {
	if len(t) > 0 {
		return t[0]
	}
	return nil
}

func (t *timerHeap) DaleyCall(interval time.Duration, cb func()) Timer {
	timer := &timer{
		trigTime:  Now().Add(interval),
		trigEvent: cb,
	}
	heap.Push(t, timer)
	return timer
}

func (t *timerHeap) IntervalCall(interval time.Duration, cb func()) Timer {
	timer := &timer{
		trigTime:  Now().Add(interval),
		trigEvent: cb,
		loopTime:  interval,
	}
	heap.Push(t, timer)
	return timer
}

func (t *timerHeap) IntervalCallWithFirst(firstInterval time.Duration, interval time.Duration, cb func()) Timer {
	timer := &timer{
		trigTime:  Now().Add(firstInterval),
		trigEvent: cb,
		loopTime:  interval,
	}
	heap.Push(t, timer)
	return timer
}

func (t *timerHeap) procTimer(now time.Time) {
	for m := t.Top(); m != nil; m = t.Top() {
		if now.Before(m.trigTime) {
			break
		}
		if m.loopTime <= 0 {
			heap.Pop(t) // 等价 heap.remove(t, m.index)
		}
		next := m.trigTime.Sub(now) + m.loopTime
		if next <= 0 {
			next = next + m.loopTime*(-next)/m.loopTime + m.loopTime
		}
		m.trigTime = now.Add(next)
		heap.Fix(t, m.index)
		// 需要在回调前处理完堆，因为回调可能会改变堆root节点
		m.trigEvent()
	}
}
