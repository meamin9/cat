package util

import (
	"encoding/json"
	"github.com/davyxu/golog"
	"os"
	"path/filepath"
)

// 打印错误，外面不要检测错误了
func LoadJson(filename string, result interface{}) {
	path := filepath.Join(Cfgpath, filename)
	f, err := os.Open(path)
	if err == nil {
		defer f.Close()
		size, _ := f.Seek(2, 0)
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

var log *Logger
var Cfgpath string

func init() {
	log = NewLog("util")
	p, _ := os.Executable()
	Cfgpath = filepath.Join(filepath.Dir(p), "appdata")
}
