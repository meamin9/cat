package chat

import (
	"time"
)

type ChannelType byte
type MemberIdType RoleIdType
type RoomId string

const (
	Channel_World ChannelType = iota
	Channel_Scene
	Channel_Team
	Channel_Friend
)

type ChatMsg struct {
	text    string
	date    time.Time
	from    MemberIdType
	to      MemberIdType
	channel ChannelType
}
type Room struct {
	members []MemberIdType
	msgs    []ChatMsg
}

type Model struct {
	rooms map[int]Room
}

func (self *Model) enterChannel(cid ChannelType, mid MemberIdType) {

}
func (self *Model) joinRoom(roomid int, memid MemberIdType) {

}

func (self *Model) addNewMsg(roomid int, msg ChatMsg) {

}
