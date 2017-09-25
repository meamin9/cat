package chat

import (
	"base"
	"time"
)

const Max_Msgs = 30

type Channel byte

const (
	Channel_World Channel = iota
	Channel_Scene
	Channel_Team
	Channel_Friend
)

type ChatMsg struct {
	text    string
	date    time.Time
	from    base.RoleId
	to      base.RoleId
	channel Channel
}
type room struct {
	memberSet map[base.RoleId]bool
	msgs      []*ChatMsg
	msgBegin  int
}

var model = struct {
	rooms map[string]room
}{
	make(map[string]room),
}

func joinRoom(roomid string, members ...base.RoleId) {
	var r room
	var ok bool
	if r, ok = model.rooms[roomid]; !ok {
		r = room{
			make(map[base.RoleId]bool, 2),
			make([]*ChatMsg, 10),
			0,
		}
		model.rooms[roomid] = r
	}
	for _, id := range members {
		r.memberSet[id] = true
	}
}

func exitRoom(roomid string, members ...base.RoleId) {
	if r, ok := model.rooms[roomid]; ok {
		for _, id := range members {
			delete(r.memberSet, id)
		}
	}
}

func deleteRoom(roomid string) {
	delete(model.rooms, roomid)
}

func addNewMsg(roomid string, msg *ChatMsg) {
	if r, ok := model.rooms[roomid]; ok {
		if len(r.msgs) == Max_Msgs {
			r.msgs[r.msgBegin] = msg
			r.msgBegin = (r.msgBegin + 1) % Max_Msgs
			if cap(r.msgs) > Max_Msgs {
				msgs := make([]*ChatMsg, Max_Msgs)
				copy(msgs, r.msgs)
				r.msgs = msgs
			}
		} else {
			r.msgs = append(r.msgs, msg)
		}
	}
}
