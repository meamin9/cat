package db

import (
	"fmt"
	"testing"
	"time"
)

func Test_Account(t *testing.T) {
	for i := 0; i < 5; i += 1 {
		j := i
		Send(func() {
			ac := Account{
				Name:    fmt.Sprintf("test%02d", j),
				Pwd:     "123456",
				RegDate: time.Now().String(),
			}
			err := AccountRegister(&ac)
			fmt.Println("result err ", err)
		})
	}
	for i := 0; i < 6; i += 1 {
		j := i
		Send(func() {
			ac, err := AccountLogin(fmt.Sprintf("test%02d", j), "123456")
			fmt.Println(ac, err)
			if err != nil {
				fmt.Println("err ", err)
			}
		})
	}
	Finish()
	//time.Sleep(5 * time.Second)
}
