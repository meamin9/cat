package mosaic

// 一些共用的数据结构会放这里，不能
import (
	"proto"
	"time"
)

type EGender int

const (
	Female EGender = iota
	Male
)

type EJob int

const (
	None EJob = iota
)

type RoleInfo struct {
	Id         uint64
	Name       string
	Gender     EGender
	Job        EJob
	Level      int
	LogoutTime time.Time
}

// 打包到网络发送
func (self *RoleInfo) PackMsg() *proto.RoleInfo {
	return &proto.RoleInfo{
		Id:         self.Id,
		Name:       self.Name,
		LogoutTime: self.LogoutTime.Unix(),
		Gender:     int32(self.Gender),
		Level:      int32(self.Level),
		Job:        int32(self.Job),
	}
}
