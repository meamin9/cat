package account

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

// Collection : Account 账户，一个账户可以创建多个角色

type dbAccount struct {
	Id    string          `bson:"_id"`
	Pwd   string          `bson:"pwd"`
	Ctime time.Time       `bson:"ctime"`
	Roles []bson.ObjectId `bson:"roles"`
}

func CName() string {
	return "accounts"
}

func Collection() *mgo.Collection {
	return db.Instance.DB().C(CName())
}

type dbAccountRegister struct {
	id string
	pwd  string
}

func (self *dbAccountRegister) Exec() (account interface{}, err error) {
	info := bson.M{
		"_id":   self.id,
		"pwd":   self.pwd,
		"ctime": time.Now(),
	}
	//c := Collection()
	//if err = c.Insert(info); err == nil {
	//	account = make(map[string]interface{})
	//	c.FindId(self.id).One(account)
	//}
	err = Collection().Insert(info)
	return account, err
}

// 如果登录成功，返回该账号的所有角色基本信息列表（可能为空）
type dbAccountLogin struct {
	id string
	pwd  string
}

func (self *dbAccountLogin) Exec() (account interface{}, err error) {
	account = make(map[string]interface{})
	err = Collection().Find(bson.M{"_id": self.id, "pwd": self.pwd}).One(account)
	if err != nil {
		return nil, err
	}
	return account, err
}

//func AccountUpdateRoles(name string, roles []int64) (data interface{}, rc error) {
//	err := Collection().UpdateId(name, bson.M{"$set": bson.M{"roles": roles}})
//	return nil, err
//}

//func InitDb() {
//	index := mgo.Index{
//		Key:        []string{"name"},
//		Unique:     true,
//		DropDups:   true,
//		Background: false,
//		Sparse:     true,
//	}
//	Collection().EnsureIndex(index)
//}
