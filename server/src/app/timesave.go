package app

import "app/db"

// 定时存档
type ITimeSaveMaster interface {
	TimeSave()
	ForceSave()
	SetDirty()
}

type IDbPacker interface {
	Pack() db.ISql
}

// 同步定时打包存盘
type TimeSaveHelper struct {
	master IDbPacker
	dirty bool
}

func (self *TimeSaveHelper) Start() {

}

func (self *TimeSaveHelper) Stop() {}

func (self *TimeSaveHelper) Save() {
	if self.dirty {
		sql := self.master.Pack()
		db.Instance.Send(&db.Mail{
			Sql: sql,
		})
	}
}

func NewTimeSaveHelper(master *IDbPacker) *TimeSaveHelper {
	return &TimeSaveHelper{
		master: master,
	}
}
