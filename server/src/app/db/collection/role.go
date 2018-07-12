package collection

import (
	//"app/account"
	"app/db"
	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
	"time"
)

// 数据表描述结构
type DbRole struct {
	Id         uint64 `bson:"_id"`
	Name       string
	Gender     int
	Job        int
	Level      int
	Account    string
	LogoutTime time.Time
	CTime      time.Time
}

// 查询返回描述结构
type DbRoleInfo struct {
	Id         uint64 `bson:"_id"`
	Name       string
	Gender     int
	Job        int
	Level      int
	LogoutTime time.Time
}

// 用来查询角色基本信息
var (
	roleCName                 = "roles"
	DbDefaultRoleInfoSelector = bson.M{
		//"_id":    1,
		"name":       1,
		"gender":     1,
		"job":        1,
		"level":      1,
		"logoutTime": 1,
	}
)

type SqlQueryRoleInfo struct {
	IdList   []uint64
	Selector map[string]interface{}
}

func (self *SqlQueryRoleInfo) Exec(s *mgo.Session) (datas interface{}, err error) {
	query := bson.M{
		"_id": bson.M{"&in": self.IdList},
	}
	if self.Selector == nil {
		self.Selector = DbDefaultRoleInfoSelector
		datas = make([]*DbRoleInfo, 0)
	} else {
		datas = make([]map[string]interface{}, 0)
	}
	err = s.DB(db.Instance.Dbname).C(roleCName).Find(query).Select(self.Selector).All(datas)
	return datas, err
}

type SqlRoleCreate struct {
	Role *DbRole
}

func (self *SqlRoleCreate) Exec(s *mgo.Session) (interface{}, error) {
	d := s.DB(db.Instance.Dbname)
	err := d.C(roleCName).Insert(self.Role)
	if err == nil {
		err = d.C(accountCName).UpdateId(self.Role.Account, bson.M{
			"$addToSet": bson.M{"roles": self.Role.Id},
		})
	}
	return self.Role, err
}

func initRole() {
	index := mgo.Index{
		Key:        []string{"name"},
		Unique:     true,
		DropDups:   true,
		Background: true,
		Sparse:     true,
	}
	db.Instance.C(roleCName, nil).EnsureIndex(index)
}
