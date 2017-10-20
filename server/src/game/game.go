package game

import (
	"db"
	_ "login"
	"network"
)

func Run() {
	db.Queue().Start() // 开启数据库协程
	network.Start()    // 注册完所有协议后开始监听网络（模块初始化时自动注册协议）
	mainLoop()
}

func mainLoop() {
	for {
		network.Queue().Poll()
		db.Queue().Poll()
	}
}
