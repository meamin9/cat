package mosaic

import (
	"app/fw"
	"app/util"
	"path/filepath"
)

type Mosaic struct {
}

func (mos *Mosaic) Load() {
	path := filepath.Join(fw.Datapath, "constants.csv")
	util.LoadCsvTranspose(path, &Const)
}

func New() *Mosaic {
	return &Mosaic{}
}
