package role

import (
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
	"app/account"
)

type DbRole struct {
	Id uint64 `bson:"_id"`
	Name string
	Job int
	Gender int
	Level int
	Account string
	MTime time.Time `bson:"mtime"`
	CTime time.Time
}

// 用来查询角色基本信息
var (
	CName = "roles"
	DbDefaultRoleInfoSelector = bson.M{
		//"_id":    1,
		"name":   1,
		"gender": 1,
		"job":    1,
		"level":  1,
		"mtime":  1,
	}
)

type DbQueryRoleInfo struct {
	IdList []uint64
	Selector map[string]interface{}
}

func (self *DbQueryRoleInfo) Exec(s *mgo.Session) (datas interface{}, err error) {
	query := bson.M{
		"_id": bson.M{"&in": self.IdList},
	}
	if self.Selector == nil {
		self.Selector = DbDefaultRoleInfoSelector
		datas = make([]*RoleInfo, 0)
	} else {
		datas = make([]map[string]interface{}, 0)
	}
	err = s.DB(db.Instance.DBName()).C(CName).Find(query).Select(self.Selector).All(datas)
	return datas, err
}

type dbRoleCreate struct {
	role *DbRole
}

func (self *dbRoleCreate) Exec(s *mgo.Session) (interface{}, error) {
	d := s.DB(db.Instance.DBName())
	err := d.C(CName).Insert(self.role)
	if err == nil {
		err = d.C(account.CName).UpdateId(self.role.Account, bson.M{
			"$addToSet": bson.M{"roles": self.role.Id },
		})
	}
	return self.role, err
}

//
//func RoleLoad(id int64) (interface{}, error) {
//	data := make(map[string]interface{}, 10)
//	//err := roleC().FindId(id).One(data)
//	return data, err
//}

func dbInit() {
	index := mgo.Index{
		Key:        []string{"name"},
		Unique:     true,
		DropDups:   true,
		Background: true,
		Sparse:     true,
	}
	db.Instance.C(CName, nil).EnsureIndex(index)
}
