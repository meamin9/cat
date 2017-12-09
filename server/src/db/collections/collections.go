package collections

import (
	"github.com/davyxu/golog"
)

var log *golog.Logger = golog.New("collections")

func printResult(data interface{}, err error) {
	if err == nil {
		log.Debugln("quest succeed", data)
	} else {
		log.Debugln("quest failed", data, err)
	}
}
