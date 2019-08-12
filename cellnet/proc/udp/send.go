package udp

import (
	"cellnet"
	"cellnet/codec"
	"cellnet/peer/udp"
	"encoding/binary"
)

const headerSize = 2 + 2

func sendPacket(writer udp.DataWriter, ctx cellnet.ContextSet, msg interface{}) error {

	// 将用户数据转换为字节数组和消息ID
	msgData, meta, err := codec.EncodeMessage(msg, ctx)

	if err != nil {
		log.Errorf("send message encode error: %s", err)
		return err
	}

	pktData := make([]byte, headerSize+len(msgData))

	// 写入消息长度做验证
	binary.LittleEndian.PutUint16(pktData, uint16(headerSize+len(msgData)))

	// Type
	binary.LittleEndian.PutUint16(pktData[2:], uint16(meta.ID))

	// Value
	copy(pktData[headerSize:], msgData)

	writer.WriteData(pktData)

	codec.FreeCodecResource(meta.Codec, msgData, ctx)

	return nil
}
