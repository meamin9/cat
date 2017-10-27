package main

import (
	"db"
	"network"
)

func main() {
	network.Peer.Queue().Wait()
}

func mainLoop() {
	for {
		network.Queue().Poll()
		db.Queue().Poll()
	}
}
