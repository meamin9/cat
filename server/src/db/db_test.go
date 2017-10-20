package db

import (
	"fmt"
	"gopkg.in/mgo.v2/bson"
	"testing"
	"time"
)

func Test_Db(t *testing.T) {
	InstallTest()
	for i := 0; i < 1; i += 1 {
		j := i
		Queue().Send(&Request{
			Quest: func() (interface{}, RetCode) {
				ac := bson.M{
					"_id":  fmt.Sprintf("test%02d", j),
					"key":  214,
					"date": time.Now(),
				}
				err := DB().C("test").Insert(ac)
				return nil, ToRetCode(err)
			},
			Result: func(data interface{}, rc RetCode) {
				fmt.Println("result code:", rc)
				if rc == CodeSuccess {
					Queue().Send(&Request{
						Quest: func() (interface{}, RetCode) {
							r := struct{ Key int64 }{}
							err := DB().C("test").Find(bson.M{
								"_id": fmt.Sprintf("test%02d", j),
							}).One(&r)
							return r, ToRetCode(err)
						},
						Result: func(data interface{}, rc RetCode) {
							d := data.(struct{ Key int64 })
							if d.Key != 214 {
								t.Error("insert find failed", d, data)
							}
						},
					})
				}
			},
		})
	}
	StopTest()
}
