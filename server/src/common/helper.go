package common

import (
	"proto"
)

func NewNoticeMsg(index int32, args ...string) *proto.SCNotice {
	return &proto.SCNotice{
		Index: index,
		Args:  args,
	}
}
