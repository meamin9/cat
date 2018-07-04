package role

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
)

type DbRole struct {
	Id bson.ObjectId `bson:"_id"`
}

func CName() string {
	return "roles"
}

func dbCollection() *mgo.Collection {
	return db.Instance.DB().C(CName())
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

type FindRoleInfo struct {

}

func (self *FindRoleInfo) Exec() (interface{}, error) {
	//DbRoleC().Find()
	return nil, nil
}



func RoleCreate(data map[string]interface{}) (result interface{}, err error) {
	//err = roleC().Insert(data)
	return data, err
}

func RoleDelete(id int64) (interface{}, error) {
	//err := roleC().RemoveId(id)
	return nil, err
}

func RoleBaseQuery(id []int64) (result interface{}, ret error) {
	//req := bson.M{"_id": bson.M{"$in": id}}
	//data := make([]map[string]interface{}, len(id))
	//err := roleC().Find(req).Select(roleBaseSelector).All(data)
	return nil, nil
}

func RoleLoad(id int64) (interface{}, error) {
	data := make(map[string]interface{}, 10)
	//err := roleC().FindId(id).One(data)
	return data, err
}

func dbInit() {
	index := mgo.Index{
		Key:        []string{"name"},
		Unique:     true,
		DropDups:   true,
		Background: true,
		Sparse:     true,
	}
	dbCollection().EnsureIndex(index)
}
