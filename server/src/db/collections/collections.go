package collections

import (
	"db"
	"github.com/davyxu/golog"
)

var log *golog.Logger = golog.New("collections")

func toRetCode(err error) db.RetCode {
	if err == nil {
		return db.CodeFailed
	}
	return db.CodeSuccess
}
