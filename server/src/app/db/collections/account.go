package collections

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

// Collection : Account 账户，一个账户可以创建多个角色

type Account struct {
	Id      string    `bson:"_id"`
	Pwd     string    `bson:"pwd"`
	RegDate time.Time `bson:"reg"`
	Roles   []int64   `bson:"rls"`
}

func accountC() *mgo.Collection {
	return db.DB().C("accounts")
}

func AccountRegister(name, pwd string) (result interface{}, rc error) {
	account := bson.M{
		"_id":     name,
		"pwd":     pwd,
		"regDate": time.Now(),
	}
	err := accountC().Insert(account)
	return account, err
}

// 如果登录成功，返回该账号的所有角色基本信息列表（可能为空）
func AccountLogin(name, pwd string) (data interface{}, rc error) {
	account := Account{}
	err := accountC().Find(bson.M{"_id": name, "pwd": pwd}).One(&account)
	if err != nil {
		return nil, err
	}
	return RoleBaseQuery(account.Roles)
}

func AccountUpdateRoles(name string, roles []int64) (data interface{}, rc error) {
	err := accountC().UpdateId(name, bson.M{"$set": bson.M{"rls": roles}})
	return nil, err
}
