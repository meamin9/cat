package collections

import (
	"db"
	"gopkg.in/mgo.v2/bson"
)

const cname = "roles"

type RoleBase struct {
	Id    int64  `bson:"_id"`
	Name  string `bson:"nam"`
	Sex   byte
	Job   byte
	Level int16 `bson:"lev"`
}

type Role struct {
	RoleBase
	Birth string `bson:"bir"`
}

func RoleCreate(name string, sex, job byte) (result interface{}, rc db.RetCode) {
	//	db.DB().C(cname).Upsert(bson.M{}, bson.M{"$setOnInsert": {"nam": name}})
}
