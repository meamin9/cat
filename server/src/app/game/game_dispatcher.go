package game

import (
	"app/network"
	"github.com/davyxu/cellnet"
	"proto"
)

func init() {
	log.Debugln("register echo")
	consts.RegisterProto("proto.Echo", func(ev *cellnet.Event) {
		msg := ev.Msg.(*proto.Echo)
		log.Debugln("rec echo", msg.Content)
		ack := proto.Echo{
			Content: msg.Content,
		}
		ev.Send(&ack)
	})
	consts.RegisterProto("coredef.SessionClosed", func(ev *cellnet.Event) {

	})

}
