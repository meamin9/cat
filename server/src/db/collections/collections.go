package collections

import (
	"db"
	"github.com/davyxu/golog"
)

var log *golog.Logger = golog.New("collections")

func printResult(data interface{}, rc db.RetCode) {
	if rc == db.CodeSuccess {
		log.Debugln("quest succeed", data)
	} else {
		log.Debugln("quest failed", data)
	}
}
