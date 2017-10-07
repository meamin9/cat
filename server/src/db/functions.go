package db

import (
	"encoding/json"
	"github.com/garyburd/redigo/redis"
)

// global tbl
const (
	accounts = "accounts"
	roles    = "r:"
)

// MARK: account data
type Account struct {
	//	Id      string `redis:"id"`
	Pwd     string  `json:"pwd"`
	RegDate string  `json:"reg"`
	Roles   []int64 `json:"rls"`
}

func QuerryAccount(c redis.Conn, id string) *Account {
	jstr, err := redis.Bytes(c.Do("HGET", accounts, id))
	if err != nil {
		log.Errorln(err)
		return nil
	}
	account := &Account{}
	if err = json.Unmarshal(jstr, account); err != nil {
		log.Errorln(err, jstr)
		return nil
	}
	return account
}

func InsertAccount(c redis.Conn, id string, account *Account) (exist bool) {
	jstr, err := json.Marshal(account)
	if err != nil {
		log.Errorln(err)
		return false
	}
	ret, err := c.Do("HSETNX", accounts, id, jstr)
	if err != nil {
		log.Errorln(err)
		return false
	}
	return ret == 0
}

// MARK: roles

type rolebase struct {
	id int64
}

func QuerryRoleBase(c redis.Conn) {

}
