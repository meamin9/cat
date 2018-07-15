package mosaic

// 一些共用的数据结构会放这里，不能
import (
	"proto"
	"time"
)

// 性别
type EGender int

const (
	Female EGender = iota
	Male
)

// 职业
type EJob int

const (
	None EJob = iota
)

// 角色信息简介
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

// 闭区间
type ClosedInterval [2]int

func (inter *ClosedInterval) InRange(n int) bool {
	return inter[0] <= n && n <= inter[1]
}
