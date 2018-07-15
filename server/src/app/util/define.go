package util

import "github.com/davyxu/golog"

var log = NewLog("util")


type Logger struct {
	*golog.Logger
}

func NewLog(tag string) *Logger {
	return &Logger{golog.New(tag)}
}
