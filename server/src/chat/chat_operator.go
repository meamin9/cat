package chat

func getRoomId(channel ChannelType) RoomId {
	return RoomId(channel)
}

// getRoomid Friend channel chat room id
func getRoomId(fromid, toid MemberIdType) RoomId {
	if fromid < toid {
		return RoomId(fromid) + RoomId(toid)
	} else {
		return RoomId(toid) + RoomId(fromid)
	}
}

func addNewTextMsg(role, content, channel) {

}
