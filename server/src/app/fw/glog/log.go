package glog

import (
	"github.com/davyxu/golog"
)

var Log = NewLog("glog") // 全局的Log， 方便使用


type Logger struct {
	*golog.Logger
}

func NewLog(tag string) *Logger {
	return &Logger{golog.New(tag)}
}

func Debugf(format string, v ...interface{}) {
	Log.Debugf(format, v...)
}

func Infof(format string, v ...interface{}) {
	Log.Infof(format, v...)
}

func Warnf(format string, v ...interface{}) {
	Log.Warnf(format, v...)
}

func Errorf(format string, v ...interface{}) {
	Log.Errorf(format, v...)
}

func Debugln(v ...interface{}) {
	Log.Debugln(v...)
}

func Infoln(v ...interface{}) {
	Log.Infoln(v...)
}

func Warnln(v ...interface{}) {
	Log.Warnln(v...)
}

func Errorln(v ...interface{}) {
	Log.Errorln(v...)
}
