package relay

import (
	"cellnet"
	"cellnet/codec"
	_ "cellnet/codec/binary"
	"cellnet/util"
	"fmt"
	"reflect"
)

type RelayACK struct {
	MsgID     uint16
	Data      []byte
	ContextID []int64
}

func (self *RelayACK) String() string { return fmt.Sprintf("%+v", *self) }

func init() {
	cellnet.RegisterMessageMeta(&cellnet.MessageMeta{
		Codec: codec.MustGetCodec("binary"),
		Type:  reflect.TypeOf((*RelayACK)(nil)).Elem(),
		ID:    int(util.StringHash("relay.RelayACK")),
	})

}
