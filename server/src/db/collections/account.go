package collections

import (
	"db"
	"gopkg.in/mgo.v2/bson"
	"time"
)

// Collection : Account 账户，一个账户可以创建多个角色

type Account struct {
	//Id      int64  `bson:"id"`
	//Email   string `bson:"eml"`
	Name    string  `bson:"_id"`
	Pwd     string  `bson:"pwd"`
	RegDate string  `bson:"reg"`
	Roles   []int64 `bson:"rls"`
}

func AccountRegister(name, pwd string) (result interface{}, rc db.RetCode) {
	account := Account{
		Name:    name,
		Pwd:     pwd,
		RegDate: time.Now().String(),
	}
	err := db.DB().C("accounts").Insert(&account)
	return nil, toRetCode(err)
}

func AccountLogin(name, pwd string) (data interface{}, rc db.RetCode) {
	account := &Account{}
	err := db.DB().C("accounts").Find(bson.M{"_id": name, "pwd": pwd}).One(account)
	return account, toRetCode(err)
}

func AccountRoles(name string, roles []int64) (data interface{}, rc db.RetCode) {
	err := db.DB().C("account").UpdateId(name, bson.M{"rls": roles})
	return nil, toRetCode(err)
}
