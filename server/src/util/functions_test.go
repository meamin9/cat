package util

import (
	"testing"
	"log"
)

type Notice struct {
	Name string
	Content string
	Typ int
	Alias string
}

func Test_LoadCsv(t *testing.T) {
	path := "../data/notice.csv"
	data := make([]*Notice, 0)
	f := &data
	LoadCsv(path, f)
	log.Print("over", len(data), len(*f))
	for i, n := range *f {
		log.Print(i, n.Name, n.Content, n.Typ, n.Alias)
	}
}
