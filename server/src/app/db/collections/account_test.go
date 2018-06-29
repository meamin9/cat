package collections

import (
	"app/db"
	"testing"
)

// Account Test
func Test_AccountRegister(t *testing.T) {
	db.InstallTest()
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return AccountRegister("test-account", "test-pwd")
		},
		Result: printResult,
	})
	db.StopTest()
}

func Test_AccountLogin(t *testing.T) {
	db.InstallTest()
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return AccountLogin("test-account", "err-pwd")
		},
		Result: func(data interface{}, rc error) {
			if rc == nil {
				t.Error("wrong pwd login succeed")
			} else {
				log.Debugln("login failed", rc)
			}
		},
	})
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			return AccountLogin("test-account", "test-pwd")
		},
		Result: func(data interface{}, rc error) {
			if rc == nil {
				d := data.(*Account)
				log.Debugln("login succeed", d)
			} else {
				t.Error("right pwd login failed", rc)
			}
		},
	})
	db.StopTest()
}

func Test_AccountRoles(t *testing.T) {
	db.InstallTest()
	db.Queue().Send(&db.Request{
		Quest: func() (interface{}, error) {
			roles := []int64{214, 233}
			return AccountRoles("test-account", roles)
		},
		Result: printResult,
	})
	db.StopTest()
}
