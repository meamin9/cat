package chat

import (
	"fmt"
	"github.com/davyxu/cellnet"
	"network"
	_ "proto/chatproto"
)

func init() {
	network.RegisterProto("chatproto.CSChatText", dispatchChatText)
}

func dispatchChatText(ev *cellnet.Event) {

}
