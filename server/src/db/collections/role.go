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
}

type Role struct {
	Id    int64  `bson:"_id"`
	Name  string `bson:"nam"`
	Sex   byte
	Job   byte
	Level int16     `bson:"lev"`
	Birth time.Time `bson:"bir"`
}

func RoleCreate(id int64, name string, sex, job byte) (result interface{}, rc db.RetCode) {
	role := Role{
		Id:    id,
		Name:  name,
		Sex:   sex,
		Job:   job,
		Level: 0,
		Birth: time.Now(),
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
