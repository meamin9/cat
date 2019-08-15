package util

import "github.com/davyxu/golog"

var Log = NewLog("util")


type Logger struct {
	*golog.Logger
}

func NewLog(tag string) *Logger {
	return &Logger{golog.New(tag)}
}
