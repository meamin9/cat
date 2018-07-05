package account

import (
	"time"
	"gopkg.in/mgo.v2/bson"
	"gopkg.in/mgo.v2"
	"app/db"
	"app/role"
)

/*
type dbAccount struct {
	Id    string          `bson:"_id"`
	Pwd   string          `bson:"pwd"`
	Ctime time.Time       `bson:"ctime"`
	Roles []bson.ObjectId `bson:"roles"`
}
*/

// 游戏中读取的数据，一些其他数据虽然会存数据库，但并不会用到，如密码、创建世界
type DbAccount struct {
	Id    string          `bson:"_id"`
	Roles []uint64 `bson:"roles"`
}

var CName = "accounts"

type dbAccountCreate struct {
	id string
	pwd  string
}

func (self *dbAccountCreate) Exec(ses *mgo.Session) (account interface{}, err error) {
	info := bson.M{
		"_id":   self.id,
		"pwd":   self.pwd,
		"ctime": time.Now(),
	}
	err = db.Instance.C(CName, ses).Insert(info)
	return account, err
}

// 如果登录成功，返回该账号的所有角色基本信息列表（可能为空）
type dbAccountLogin struct {
	id string
	pwd  string
}

func (self *dbAccountLogin) Exec(s *mgo.Session) (data interface{}, err error) {
	account := &DbAccount{}
	d := s.DB(db.Instance.DBName())
	err = d.C(CName).Find(bson.M{"_id": self.id, "pwd": self.pwd}).One(account)
	if err != nil {
		return nil, err
	}
	q := role.DbQueryRoleInfo{
		IdList: account.Roles,
	}
	infos, err := q.Exec(s)
	if err != nil {
		return nil, err
	}
	data = &Account{
		Id: account.Id,
		Roles: infos.([]*role.RoleInfo),
	}
	return data, err
}

// 不需要更新，创建角色时，自动修改account插入数据
//type DbAccountUpdate struct {
//	datas []*DbAccount
//}
//
//func (self *DbAccountUpdate) Exec(s *mgo.Session) (interface{}, error) {
//	c := s.DB(db.Instance.DBName()).C(CName)
//	for _, acc := range self.datas {
//		c.UpdateId(acc.Id, bson.M{"roles": acc.Roles})
//	}
//	return nil, nil
//}
