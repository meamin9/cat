package user

import (
	"app/fw/glog"
	"github.com/davyxu/cellnet"
	"time"
)

type User *implementUser

type implementUser struct {
	cellnet.Session
	ctime time.Time
	account    *Account
}



type manager struct {
	userBySid map[int64]User
	procById map[int]func(User, interface{})
	*network
}

var (
	log     = glog.NewLog("user")
	Manager = &manager{
		userBySid: make(map[int64]User),
		procById:  make(map[int]func(User, interface{})),
		network: newNetWork(),
	}
)

func (m *manager) ProcNetEvent(event cellnet.Event, msgId int) {
	defer func() {
		if err := recover(); err != nil {
			log.Errorf("panic:%+v", err)
		}
	}()
	ses := event.Session()
	msg := event.Message()
	switch msg.(type) {
	default:
		if user, ok := m.userBySid[ses.ID()]; ok {
			if proc, ok := m.procById[msgId]; ok {
				proc(user, event.Message())
			} else {
				log.Warnf("unknown net msgId=%v", msgId)
			}
		} else {
			log.Errorf("not found user sessionId=%v, msgId=%v", ses.ID(), msgId)
			ses.Close()
		}
	case cellnet.SessionAccepted:
		log.Debugf("session accepted sid=%v time=%v", ses.ID(), fw.Now())
		m.userBySid[ses.ID()] = &implementUser{Session:ses, ctime: fw.Now()}
	case cellnet.SessionClosed:
		if user, ok := m.userBySid[ses.ID()]; ok {
			log.Warnf("session closed user=%v", user)
			ses.Close()
		} else {
			log.Errorf("not found user sessionId=%v, msgId=%v", ses.ID(), msgId)
			ses.Close()
		}
	}
}

func (m *manager) RegNetMsg(msgId int, proc func(User, interface{})) {
	if _, ok := m.procById[msgId]; ok {
		panic("proto is register repeated")
	}
	m.procById[msgId] = proc
}

//func newEntryTokenPair() (alias, token string) {
//	var b [8]byte
//	binary.BigEndian.PutUint32(b[:], rand.Uint32())
//	binary.BigEndian.PutUint32(b[4:], uint32(time.Now().Unix()))
//	alias = string(b[:])
//	// 计算规则
//	var hexb []byte
//	hex.Encode(hexb, b[:])
//	t := md5.Sum(hexb)
//	for i, c := range t {
//		t[i] = c ^ cryptoKey[i]
//	}
//	token = string(t[:])
//	binary.BigEndian.PutUint16(b[8:], 60)
//	return alias, token
//}

