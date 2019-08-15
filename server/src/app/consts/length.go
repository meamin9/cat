package consts

type Region struct {
	Min int
	Max int
}

func (r *Region) In(value int) bool {
	return r.Min <= value && value <= r.Max
}

var (
	AccountLen = Region{6, 24}
	RoleNameLen = Region{6,16}
)
