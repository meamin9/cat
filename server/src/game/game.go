package game

import (
	"db"
	_ "login"
	"network"
	"time"
)

var exit chan int

func Run() {
	exit = make(chan int, 1)
	db.Queue().Start() // 开启数据库协程
	network.Start()    // 注册完所有协议后开始监听网络（模块初始化时自动注册协议）
	tick := time.NewTicker(1.0 / time.Second)
MainLoop:
	for {
		select {
		case r := <-network.Queue().C():
			r()
		case r := <-db.Queue().C():
			r()
		case <-tick.C:
		case <-exit:
			break MainLoop
		}
	}
	tick.Stop()
	network.Queue().Poll() // 处理完接受队列
	network.Peer().Stop()  // 停止连接
	db.Queue().Poll()      // 先清掉已完成的db请求
	db.Queue().Stop()      // 阻塞到所有数据库请求已经完成，后面的请求将被忽略
}

func StopGame(int i) {
	exit <- i
}
