package main

import (
	_ "db/mdb"
	"network"
)

func main() {
	network.Peer.Queue().Wait()
}
