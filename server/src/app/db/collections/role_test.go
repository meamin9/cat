package collections

import (
	"app/db"
	"testing"
	"time"
)

func Test_RoleCreate(t *testing.T) {
	db.InstallTest()
	InitIndexes()
	data := map[string]interface{}{
		"_id":   1000,
		"name":  "test role",
		"birth": time.Now(),
		"level": 233,
	}
	db.Queue().Send(db.NewRequest(func() (interface{}, error) {
		return RoleDelete(1000)
	}, nil))
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return RoleCreate(data)
		},
		Result: func(d interface{}, err error) {
			t.Log(d)
		},
	})
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return RoleLoad(1000)
		},
		Result: func(d interface{}, err error) {
			t.Log(d)
		},
	})
	db.StopTest()
}
