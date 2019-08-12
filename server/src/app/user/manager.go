package user

import (
	"time"
)

type User struct {
	CreateTime time.Time
	Session    Session
	Account    Account
}


type Manager struct {

}

var _mgr = &Manager{}

func Init() {

}

func PostInit() {

}
