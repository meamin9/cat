package chat

import (
	"base"
	"fmt"
)

// getRoomid Friend channel chat room id
func getRoomId(channel Channel, to, from base.RoleId) string {
	if to >= from {
		return fmt.Sprintf("%s_%s_%s", channel, to, from)
	} else {
		return fmt.Sprintf("%s_%s_%s", channel, to, from)
	}
}
