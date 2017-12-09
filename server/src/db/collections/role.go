package collections

import (
	"db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
)

func roleC() *mgo.Collection {
	return db.DB().C("roles")
}

// 用来查询角色基本信息
var roleBaseSelector = bson.M{
	"_id":    1,
	"name":   1,
	"gender": 1,
	"job":    1,
	"level":  1,
	"birth":  1,
}

func RoleCreate(data map[string]interface{}) (result interface{}, err error) {
	err = roleC().Insert(data)
	return data, err
}

func RoleDelete(id int64) (interface{}, error) {
	err := roleC().RemoveId(id)
	return nil, err
}

func RoleBaseQuery(id []int64) (result interface{}, ret error) {
	req := bson.M{"_id": bson.M{"$in": id}}
	data := make([]map[string]interface{}, len(id))
	err := roleC().Find(req).Select(roleBaseSelector).All(data)
	return data, err
}

func RoleLoad(id int64) (interface{}, error) {
	data := make(map[string]interface{}, 10)
	err := roleC().FindId(id).One(data)
	return data, err
}

func InitIndexes() {
	index := mgo.Index{
		Key:        []string{"name"},
		Unique:     true,
		DropDups:   false,
		Background: true,
		Sparse:     true,
	}
	roleC().DropIndex("level")
	roleC().EnsureIndex(index)
}

func init() {
	InitIndexes()
}
