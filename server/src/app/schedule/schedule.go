package schedule

var timemgrIns *timemgr

func Time() *timemgr {
	if timemgrIns == nil {
		timemgrIns = NewTimemgr()
	}
	return timemgrIns
}
