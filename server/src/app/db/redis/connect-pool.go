package db

import (
	"github.com/davyxu/golog"
	"github.com/garyburd/redigo/redis"
	"time"
)

var Pool *redis.Pool
var conn redis.Conn
var log *golog.Logger = golog.New("db")

const (
	server   = "127.0.0.1"
	password = "123456"
	dbname   = "cat"
)

func connectRedis() (redis.Conn, error) {
	c, err := redis.Dial("tcp", server)
	if err != nil {
		return nil, err
	}
	if _, err := c.Do("AUTH", password); err != nil {
		c.Close()
		return nil, err
	}
	if _, err := c.Do("SELECT", dbname); err != nil {
		c.Close()
		return nil, err
	}
	return c, nil
}

func GetConn() redis.Conn {
	if conn == nil {
		var err error
		if conn, err = connectRedis(); err != nil {
			conn = nil
			log.Warnln("connect redis failed")
		}
	}
	return conn
}

func init() {
	Pool = &redis.Pool{
		MaxIdle:     100,
		IdleTimeout: time.Minute,
		Wait:        true,
		Dial:        connectRedis,
	}
	GetConn()
}
