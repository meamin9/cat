package chat

import (
	"time"
)

type ChannelType byte
type MemberIdType int64

const (
	Channel_World ChannelType = iota
	Channel_Scene
	Channel_Team
	Channel_Friend
)

type ChatMsg struct {
	text    string
	time    time.Time
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

func (self *Model) addNewMsg(mid MemberIdType) {

}
