package apptime

import (
	"time"
)

// 游戏时间和系统时间不一定相同, 时间是最基本的，不要包含其他包

type manager struct {
	offset time.Duration
	now    time.Time
	tick   *time.Ticker

	timerHeap // 提供定时器
}

var (
	Manager = &manager{
		now: time.Now(),
	}
)

func Now() time.Time {
	return Manager.now
}

func (m *manager) Start() {
	m.tick = time.NewTicker(1)
}

func (m *manager) Stop() {
	m.tick.Stop()
}

func (m *manager) TickChan() <-chan time.Time {
	return m.tick.C
}

func (m *manager) Tick(t time.Time) {
	m.now = t.Add(m.offset)
	m.procTimer(m.now)
}
