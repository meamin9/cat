package mdb

import (
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
)

const dbname = "cat"

// Collection : Account 账户，一个账户可以创建多个角色

type Account struct {
	//Id      int64  `bson:"id"`
	//Email   string `bson:"eml"`
	Name    string  `bson:"_id"`
	Pwd     string  `bson:"pwd"`
	RegDate string  `bson:"reg"`
	Roles   []int64 `bson:"rls"`
}

func AccountC(s *mgo.Session) *mgo.Collection {
	return s.DB(dbname).C("accounts")
}

func AccountRegister(account *Account) (err error) {
	ses := GetSession()
	defer ses.Close()
	err = AccountC(ses).Insert(&account)
	return
}

func AccountLogin(name, pwd string) (account *Account, err error) {
	s := GetSession()
	defer s.Close()
	account = &Account{}
	err = AccountC(s).Find(bson.M{"_id": name, "pwd": pwd}).One(account)
	return
}

// Collection : Role

// RoleBase 角色基本信息
type RoleBase struct {
	Id    int64  `bson:"_id"`
	Name  string `bson:"nam"`
	Level int16  `bson:"lev"`
	job   byte   `bson:"job"`
}

func RoleC(s *mgo.Session) *mgo.Collection {
	return s.DB(dbname).C("roles")
}

func RoleBaseQuery(ids []int64) (roles []RoleBase) {
	s := GetSession()
	defer s.Close()
	roles = make([]RoleBase, len(ids))
	return
}
