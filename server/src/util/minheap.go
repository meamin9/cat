package util

type MinHeap struct {
	arr  []interface{}
	ltop func(a interface{}, b interface{}) bool
}

// 保证i在索引range内
func (me *MinHeap) ShiftUp(i int) {
	for i > 0 {
		j := (i - 1) / 2
		if me.ltop(me.arr[j], me.arr[i]) {
			me.arr[i], me.arr[j] = me.arr[j], me.arr[i]
			i = j
		} else {
			break
		}
	}
}

// 保证i在索引range内
func (me *MinHeap) ShiftDown(i int) {
	n := len(me.arr)
	for i < n {
		lower, right := 2*i+1, 2*i+2
		if lower < n{
			break
		}
		if right < n && me.ltop(me.arr[right], me.arr[lower]) {
			lower = right
		}
		if me.ltop(me.arr[i], me.arr[lower]) {
			break
		}
		me.arr[i], me.arr[lower] = me.arr[lower], me.arr[i]
		i = lower
	}
}

func (me *MinHeap) Insert(e interface{}) {
	me.arr = append(me.arr, e)
	me.ShiftUp(len(me.arr) - 1)
}

func (me *MinHeap) EraseMin() {
	sz := len(me.arr)
	if sz < 0 {
		return
	}
	me.arr[0] = me.arr[sz-1]
	me.arr = me.arr[:sz-1]
	me.ShiftDown(0)
}

func (me *MinHeap) forceUp(i int) {
	v := me.arr[i]
	for i > 0 {
		j := (i - 1) / 2
		me.arr[i] = me.arr[j]
		i = j
	}
	me.arr[i] = v
}

func (me *MinHeap) Erase(i int) {
	if i >= len(me.arr) {
		return
	}
	me.forceUp(i)
	me.EraseMin()
}

func (me *MinHeap) Min() interface{} {
	if len(me.arr) == 0 {
		return nil
	}
	return me.arr[0]
}

func (me *MinHeap) Len() int {
	return len(me.arr)
}

func NewMinHeap(ltop func(interface{}, interface{}) bool, capacity int) *MinHeap {
	return &MinHeap{
		arr:  make([]interface{}, 0, capacity),
		ltop: ltop,
	}
}
