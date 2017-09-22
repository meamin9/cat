package main

import (
	_ "chat"
	"network"
)

func main() {
	network.Host.Queue().Wait()
}
