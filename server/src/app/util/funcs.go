package util

import (
	"encoding/json"
	"github.com/davyxu/golog"
	"os"
)

var log = NewLog("util")


// 打印错误，外面不要检测错误了
func LoadJson(path string, result interface{}) {
	f, err := os.Open(path)
	if err == nil {
		defer f.Close()
		size, _ := f.Seek(0, 2)
		b := make([]byte, size)
		f.Seek(0, 0)
		f.Read(b)
		if err = json.Unmarshal(b, result); err != nil {
			log.Errorf("load json(%v) error: %v", path, err)
		}
	} else {
		log.Errorf("open json(%v) error: %v", path, err)
	}
}

type Logger struct {
	*golog.Logger
}

func NewLog(tag string) *Logger {
	return &Logger{golog.New(tag)}
}


//func init() {
//	log = NewLog("util")
	//log.Infoln(os.Args[1], os.Args[2])
	//p, _ := os.Executable()
	//dir := os.Args
	//Jsonpath = filepath.Join(dir, "appdata")
	//Datapath = filepath.Join(dir, "")
//}
