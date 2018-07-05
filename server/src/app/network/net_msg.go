package network

import (
	"cellnet/util"
	"time"
	"sync"
	"proto"
)

func regSystemMsg() {
	// 注册网络事件
	var msgId int
	msgId = int(util.StringHash("cellnet.SessionAccepted"))
	Instance.RegProto(msgId, recvSessionAccepted)
	msgId = int(util.StringHash("cellnet.SessionClosed"))
	Instance.RegProto(msgId, recvSessionClosed)
	Instance.RegProto(proto.CodeCSEntryToken, recvEntryToken)
}

func recvSessionAccepted(s Session, data interface{}) {
	alias, token := newEntryTokenPair()
	timeout := time.AfterFunc(60, func() {
		s.Close()
		if tcp, ok := s.(interface{ WaitClose()}); ok {
			tcp.WaitClose()
		}
		Instance.sesInToken.Delete(s.ID())
	})
	entry := &entryToken{
		token: token,
		timeout: timeout,
	}
	Instance.sesInToken.Store(s.ID(), entry)
	s.Send(&proto.SCEntryToken{
		Alias: alias,
	})
}

func recvEntryToken(s Session, data interface{}) {
	entry, ok := Instance.sesInToken.Load(s.ID())
	if ok {
		en := entry.(*entryToken)
		en.timeout.Stop()
		if en.token == data.(*proto.CSEntryToken).Token {
			Instance.sesInToken.Delete(s.ID())
			return
		}
	}
	s.Close()
	if tcp, ok := s.(interface{ WaitClose()}); ok {
		tcp.WaitClose()
	}
	Instance.sesInToken.Delete(s.ID())
}

func recvSessionClosed(s Session, data interface{}) {

}

func newEntryTokenPair() (alias, token string) {
	//var b [12]byte
	//// Timestamp, 4 bytes, big endian
	//binary.BigEndian.PutUint32(b[:], uint32(time.Now().Unix()))
	//binary.BigEndian.PutUint16(b[4:], 60)
	return "alias", "cat"
}

type entryToken struct {
	token string
	timeout *time.Timer
}

type tokenMgr struct {
	sesInCheck map[int64]*entryToken
	sesInToken sync.Map

}
