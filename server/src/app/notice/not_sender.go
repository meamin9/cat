package notice

import (
	"app/user"
	"fmt"
	"proto"
)

func SendNotice(sender user.ISender, noticeId int, args ...interface{}) {
	var strs []string
	if args != nil {
		strs = make([]string, len(args))
		for i, arg := range args {
			strs[i] = fmt.Sprintf("%v", arg)
		}
	}
	sender.Send(&proto.SCNotice{
		Index: int32(noticeId),
		Args:  strs,
	})
}

func SendNoticeText(sender user.ISender, text string) {
	SendNoticeTextByType(sender, text, 0)
}

func SendNoticeTextByType(sender user.ISender, text string, typ int) {
	sender.Send(&proto.SCNoticeText{
		Text: text,
		Typ:  int32(typ),
	})
}
