package main

import (
	"db/mdb"
	"network"
)

func main() {
	network.Host.Queue().Wait()
}
