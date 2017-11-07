package collections

import (
	"db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

// Collection : Account 账户，一个账户可以创建多个角色

type Account struct {
	//Id      int64  `bson:"id"`
	//Email   string `bson:"eml"`
	Name    string    `bson:"_id"`
	Pwd     string    `bson:"pwd"`
	RegDate time.Time `bson:"reg"`
	Roles   []int64   `bson:"rls"`
}

func accountC() *mgo.Collection {
	return db.DB().C("accounts")
}

func AccountRegister(name, pwd string) (result interface{}, rc db.RetCode) {
	account := Account{
		Name:    name,
		Pwd:     pwd,
		RegDate: time.Now(),
	}
	err := accountC().Insert(&account)
	return &account, db.ToRetCode(err)
}

// 如果登录成功，返回该账号的所有角色基本信息列表（可能为空）
func AccountLogin(name, pwd string) (data interface{}, rc db.RetCode) {
	account := Account{}
	err := accountC().Find(bson.M{"_id": name, "pwd": pwd}).One(&account)
	if err != nil {
		return nil, db.ToRetCode(err)
	}
	return RoleBaseQuery(account.Roles)
}

func AccountRoles(name string, roles []int64) (data interface{}, rc db.RetCode) {
	err := accountC().UpdateId(name, bson.M{"$set": bson.M{"rls": roles}})
	return nil, db.ToRetCode(err)
}
