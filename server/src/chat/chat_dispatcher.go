package chat

import (
	"fmt"
	"github.com/davyxu/cellnet"
	"network"
	_ "proto/chatproto"
)

func init() {
	// register chat proto
	network.RegisterProto("chatproto.CSChatText", dispatchChatText)
	fmt.Println("Initialize chat")
}

func dispatchChatText(*cellnet.Event) {
}
