package account

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

// Collection : Account 账户，一个账户可以创建多个角色

type dbAccount struct {
	Name      string    `bson:"_id"`
	Pwd     string    `bson:"pwd"`
	RegTime time.Time `bson:"regTime"`
	Roles   []int64   `bson:"roles"`
}

func collection() *mgo.Collection {
	return db.Svc.DB().C("accounts")
}

type dbAccountRegister struct {
	name string
	pwd string
}

func (self *dbAccountRegister) Exec() (account interface{}, err error) {
	regTime := time.Now()
	info := bson.M{
		"_id":     self.name,
		"pwd":     self.pwd,
		"regTime": regTime,
	}
	err = collection().Insert(info)
	if err == nil {
		account = &dbAccount{
			self.name, self.pwd, regTime, nil,
		}
	}
	return account, err
}

// 如果登录成功，返回该账号的所有角色基本信息列表（可能为空）
type dbAccountLogin struct {
	name string
	pwd string
}

func (self *dbAccountLogin) Exec() (account interface{}, err error) {
	account = &dbAccount{}
	err = collection().Find(bson.M{"_id": self.name, "pwd": self.pwd}).One(account)
	if err != nil {
		return nil, err
	}
	return account, err
}

func AccountUpdateRoles(name string, roles []int64) (data interface{}, rc error) {
	err := collection().UpdateId(name, bson.M{"$set": bson.M{"roles": roles}})
	return nil, err
}
