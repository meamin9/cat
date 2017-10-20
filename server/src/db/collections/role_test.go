package collections

import (
	"db"
	"testing"
)

func Test_RoleCreate(t *testing.T) {
	db.InstallTest()
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, db.RetCode) {
			return RoleCreate("rolename", 1, 0)
		},
		Result: printResult,
	})
	db.StopTest()
}
