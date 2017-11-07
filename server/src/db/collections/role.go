package collections

import (
	"db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

func roleC() *mgo.Collection {
	return db.DB().C("roles")
}

type RoleBase struct {
	Id     int64     `bson:"_id"`
	Name   string    `bson:"nam"`
	Gender byte      `bson:"gen"`
	Job    byte      `bson:"job"`
	Level  int16     `bson:"lev"`
	Birth  time.Time `bson:"bir"`
}

// 用来查询角色基本信息
var roleBaseSelector bson.M = bson.M{
	"_id": 1,
	"nam": 1,
	"gen": 1,
	"job": 1,
	"lev": 1,
	"bir": 1,
}

type Role struct {
	Id     int64     `bson:"_id"`
	Name   string    `bson:"nam"`
	Gender byte      `bson:"gen"`
	Job    byte      `bson:"job"`
	Level  int16     `bson:"lev"`
	Birth  time.Time `bson:"bir"`
}

func RoleCreate(id int64, name string, gender, job byte) (result interface{}, rc db.RetCode) {
	role := Role{
		Id:     id,
		Name:   name,
		Gender: gender,
		Job:    job,
		Level:  0,
		Birth:  time.Now(),
	}
	info, err := roleC().Upsert(bson.M{"nam": name}, bson.M{"$setOnInsert": &role})
	if err != nil {
		rc = db.CodeError
	} else if info.Matched >= 0 {
		rc = db.CodeAlreadyExist
	} else {
		rc = db.CodeSuccess
	}
	return &role, rc
}

func RoleBaseQuery(id []int64) (result interface{}, ret db.RetCode) {
	req := bson.M{"_id": bson.M{"$in": id}}
	var data []RoleBase = make([]RoleBase, 0)
	err := roleC().Find(req).Select(roleBaseSelector).All(data)
	return data, db.ToRetCode(err)
}

func init() {
	index := mgo.Index{
		Key:        []string{"nam"},
		Unique:     true,
		DropDups:   true,
		Background: true,
		Sparse:     true,
	}
	roleC().EnsureIndex(index)
}
