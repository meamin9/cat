package collection

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

var accountCName = "accounts"

/*
type dbAccountTrue struct {
	Id    string          `bson:"_id"`
	Pwd   string          `bson:"Pwd"`
	Ctime time.Time       `bson:"ctime"`
	Roles []uint64 `bson:"roles"`
}
*/

// 游戏中读取的数据，一些其他数据虽然会存数据库，但并不会用到，如密码、创建世界
type dbAccountBase struct {
	Id    string   `bson:"_id"`
	Roles []uint64 `bson:"roles"`
}

type DbAccountRet struct {
	Id    string `bson:"_id"`
	Roles []*DbRoleInfo
}

// 1. 创建账号
type SqlAccountCreate struct {
	Id  string
	Pwd string
}

func (self *SqlAccountCreate) Exec(ses *mgo.Session) (account interface{}, err error) {
	info := bson.M{
		"_id":   self.Id,
		"Pwd":   self.Pwd,
		"ctime": time.Now(),
	}
	err = ses.DB(db.Instance.Dbname).C(accountCName).Insert(info)
	return account, err
}

// 如果登录成功，返回该账号的所有角色基本信息列表（可能为空）
type SqlAccountLogin struct {
	Id  string
	Pwd string
}

func (self *SqlAccountLogin) Exec(s *mgo.Session) (data interface{}, err error) {
	account := &dbAccountBase{}
	d := s.DB(db.Instance.Dbname)
	err = d.C(roleCName).Find(bson.M{"_id": self.Id, "Pwd": self.Pwd}).One(account)
	if err != nil {
		return nil, err
	}
	q := SqlQueryRoleInfo{
		IdList: account.Roles,
	}
	infos, err := q.Exec(s)
	if err != nil {
		return nil, err
	}
	data = &DbAccountRet{
		Id:    account.Id,
		Roles: infos.([]*DbRoleInfo),
	}
	return data, err
}
