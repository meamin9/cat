package user

import (
	"app/fw/appinfo"
	"app/fw/glog"
	"github.com/davyxu/cellnet"
	"math/rand"
	"time"
)

type User *implementUser

type implementUser struct {
	cellnet.Session
	ctime time.Time
	account    string
}

type manager struct {
	userByRid      map[int]User
	*network
}

var (
	log     = glog.NewLog("user")
	Manager = &manager{
		userByRid:      make(map[int]User),
		network:        newNetWork(),
	}
)

func newRoleId() uint64 {
	time := time.Now().Unix() // 时间不回调基本不会冲突
	serverId := uint16(appinfo.ServerId)
	random := rand.Uint32()
	return uint64(time)<<32 | uint64(serverId)<<16 | uint64(random)
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

