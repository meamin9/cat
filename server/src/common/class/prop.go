package class

type IProp interface {
	Get(int) interface{}
	GetInt(int) int
	GetInt16(int) int16
	GetInt32(int) int32
	GetString(int) string
}

type prop struct {
	a [34]int
}
