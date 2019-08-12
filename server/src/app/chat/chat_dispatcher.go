package chat

import (
	"app/network"
	"app/role"
	"github.com/davyxu/cellnet"
	"fmt"
	_ "proto/chatproto"
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
