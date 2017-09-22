package role

type IdType int64
type SexType byte

const (
	Female SexType = iota
	Male
)

type RoleBaseInfo struct {
	Id    IdType
	Name  string
	Level int16
	Sex   SexType
}

type RoleProfile struct {
	RoleBaseInfo
}

type Role struct {
	RoleProfile
}
