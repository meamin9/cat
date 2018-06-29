package collections

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
)

type IdSeeds struct {
	Role int64
	Item int64
}

func idC() *mgo.Collection {
	return db.DB().C("ids")
}

func init() {
	seed := IdSeeds{
		Role: 100000,
		Item: 10000000,
	}
	idC().Upsert(bson.M{}, bson.M{"setOnInsert": &seed})
}

func IdLoad() (interface{}, error) {
	ids := IdSeeds{}
	err := idC().Find(nil).One(&ids)
	return &ids, err
}

func genId(name string, result interface{}) {
	change := mgo.Change{
		Update:    bson.M{"$inc": bson.M{name: 1}},
		ReturnNew: true,
	}
	info, _ := idC().Find(nil).Apply(change, result)
	log.Debugln("genId ret ", info)
}

func genRoleId() int64 {
	r := struct{ Role int64 }{}
	genId("role", &r)
	return r.Role
}

func genItemId() int64 {
	r := struct{ Item int64 }{}
	genId("item", &r)
	return r.Item
}
