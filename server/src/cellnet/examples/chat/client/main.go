package main

import (
	"bufio"
	"cellnet"
	"cellnet/examples/chat/proto/chatproto"
	"cellnet/socket"
	"github.com/davyxu/golog"
	"os"
	"strings"
)

var log *golog.Logger = golog.New("main")

func ReadConsole(callback func(string)) {

	for {
		text, err := bufio.NewReader(os.Stdin).ReadString('\n')
		if err != nil {
			break
		}
		text = strings.TrimRight(text, "\n\r ")

		text = strings.TrimLeft(text, " ")

		callback(text)
	}
}

func main() {
	queue := cellnet.NewEventQueue()

	peer := socket.NewConnector(queue).Start("127.0.0.1:8801")
	peer.SetName("client")

	cellnet.RegisterMessage(peer, "chatproto.ChatACK", func(ev *cellnet.Event) {
		msg := ev.Msg.(*chatproto.ChatACK)

		log.Infof("sid%d say: %s", msg.Id, msg.Content)
	})

	queue.StartLoop()

	ReadConsole(func(str string) {

		peer.(socket.Connector).DefaultSession().Send(&chatproto.ChatREQ{
			Content: str,
		})
	})
}
