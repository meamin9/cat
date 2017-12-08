package game

import (
	"cellnet"
	"network"
	"proto"
)

func init() {
	log.Debugln("register echo")
	network.RegisterProto("proto.Echo", func(ev *cellnet.Event) {
		msg := ev.Msg.(*proto.Echo)
		log.Debugln("rec echo", msg.Content)
		ack := proto.Echo{
			Content: msg.Content,
		}
		ev.Send(&ack)
	})
	network.RegisterProto("coredef.SessionClosed", func(ev *cellnet.Event) {

	})

}
