package network

import (
	"cellnet/util"
	"time"
	"proto"
	"cellnet/peer"
	"encoding/binary"
	"math/rand"
	"encoding/hex"
	"crypto/md5"
)

var cryptoKey = "milan, milan go!"

type entryToken struct {
	Token string
	Timeout *time.Timer
	Ok bool // 合法的连接这个值要为true
}
var (
	CodeSessionAccepted = int(util.StringHash("cellnet.SessionAccepted"))
	CodeSessionClosed = int(util.StringHash("cellnet.SessionClosed"))
)
func regSystemMsg() {
	// 注册网络事件
	//Instance.RegProto(CodeSessionAccepted, recvSessionAccepted)
	Instance.RegProto(CodeSessionClosed, recvSessionClosed)
	// 连接口令
	//Instance.RegProto(proto.CodeCSEntryToken, recvEntryToken)
}

// 这个信息是io线程并发的
func recvSessionAccepted(s Session, _ interface{}) {
	// 记录在session上
	alias, token := newEntryTokenPair()
	timeout := time.AfterFunc(30, func() { // 30秒内没收到口令直接断开
		s.Close()
		//if tcp, ok := s.(interface{ WaitClose()}); ok {
		//	tcp.WaitClose()
		//}
		//Instance.sesInToken.Delete(s.ID())
	})
	entry := &entryToken{
		Token: token,
		Timeout: timeout,
	}
	//Instance.sesInToken.Store(s.ID(), entry)
	s.(peer.ICoreContextSet).SetContext("token", entry)
	sendEntryAlias(s, alias)
}

func sendEntryAlias(sender Session, alias string) {
	sender.Send(&proto.SCEntryAlias{
		Alias: alias,
	})
}

// 这个信息是io线程并发的
func recvEntryToken(s Session, data interface{}) {
	//entry, ok := Instance.sesInToken.Load(s.ID())
	entry, ok := s.(peer.ICoreContextSet).RawGetContext("token")
	if ok {
		en := entry.(*entryToken)
		en.Timeout.Stop()
		if en.Token == data.(*proto.CSEntryToken).Token {
			//Instance.sesInToken.Delete(s.ID())
			en.Ok = true
			return
		}
	}
	s.Close()
	//if tcp, ok := s.(interface{ WaitClose()}); ok {
	//	tcp.WaitClose()
	//}
	//Instance.sesInToken.Delete(s.ID())
}

func recvSessionClosed(s Session, data interface{}) {

}

func newEntryTokenPair() (alias, token string) {
	var b [8]byte
	binary.BigEndian.PutUint32(b[:], rand.Uint32())
	binary.BigEndian.PutUint32(b[4:], uint32(time.Now().Unix()))
	alias = string(b[:])
	// 计算规则
	var hexb []byte
	hex.Encode(hexb, b[:])
	t := md5.Sum(hexb)
	for i, c := range t {
		t[i] = c ^ cryptoKey[i]
	}
	token = string(t[:])
	binary.BigEndian.PutUint16(b[8:], 60)
	return alias, token
}

