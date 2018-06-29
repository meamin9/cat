package chat

import (
	"cellnet"
	"fmt"
	"app/network"
	_ "proto/chatproto"
	"app/role"
	"time"
)

func init() {
	network.RegisterProto("chatproto.CSChatText", dispatchChatText)
}

func dispatchChatText(ev *cellnet.Event) {
	proto := ev.Msg.(*chatproto.CSChatText)
	r := role.getRoleBySid(ev.SessionID())
	msg := chatMsg{
		content: proto.Content,
		date:    time.Now(),
		from:    r.Id(),
		channel: proto.Channel,
	}

}
