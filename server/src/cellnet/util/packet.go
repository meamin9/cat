package util

import (
	"encoding/binary"
	"errors"
	"cellnet"
	"cellnet/codec"
	"io"
)

var (
	ErrMaxPacket = errors.New("Invalid packet size")
)

// 接收Length-Type-Value格式的封包流程
func RecvLTVPacket(reader io.Reader, maxPacketSize int) (msg interface{}, err error) {

	// Size为uint16，占2字节
	var sizeBuffer = make([]byte, 2)

	// 持续读取Size直到读到为止
	_, err = io.ReadFull(reader, sizeBuffer)

	// 发生错误时返回
	if err != nil {
		return
	}

	// 用小端格式读取Size
	size := binary.LittleEndian.Uint16(sizeBuffer)

	if maxPacketSize > 0 && size >= uint16(maxPacketSize) {
		return nil, ErrMaxPacket
	}

	// 分配包体大小
	body := make([]byte, size)

	// 读取包体数据
	_, err = io.ReadFull(reader, body)

	// 发生错误时返回
	if err != nil {
		return
	}

	msgid := binary.LittleEndian.Uint16(body)

	msgData := body[2:]

	// 将字节数组和消息ID用户解出消息
	msg, _, err = codec.DecodeMessage(int(msgid), msgData)
	if err != nil {
		// TODO 接收错误时，返回消息
		return nil, err
	}

	return
}

// 发送Length-Type-Value格式的封包流程
func SendLTVPacket(writer io.Writer, ctx cellnet.ContextSet, data interface{}) error {

	// 取Socket连接
	var msgData []byte
	var msgID int

	var meta *cellnet.MessageMeta

	switch m := data.(type) {
	case *cellnet.RawPacket: // 发裸包
		msgData = m.MsgData
		msgID = m.MsgID
	default: // 发普通编码包
		var err error

		// 将用户数据转换为字节数组和消息ID
		msgData, meta, err = codec.EncodeMessage(data, ctx)

		if err != nil {
			return err
		}

		msgID = meta.ID
	}

	pkt := make([]byte, 2+2+len(msgData))

	// Length
	binary.LittleEndian.PutUint16(pkt, uint16(2+len(msgData)))

	// Type
	binary.LittleEndian.PutUint16(pkt[2:], uint16(msgID))

	// Value
	copy(pkt[2+2:], msgData)

	// 将数据写入Socket
	err := WriteFull(writer, pkt)

	// Codec中使用内存池时的释放位置
	if meta != nil {
		codec.FreeCodecResource(meta.Codec, msgData, ctx)
	}

	return err
}
