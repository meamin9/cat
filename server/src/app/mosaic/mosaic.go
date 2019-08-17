package mosaic

import (
	"app/fw"
	util2 "app/fw/util"
	"path/filepath"
)

type Mosaic struct {
}

func (mos *Mosaic) Load() {
	path := filepath.Join(fw.Datapath, "constants.csv")
	util2.LoadCsvTranspose(path, &Const)
}

func New() *Mosaic {
	return &Mosaic{}
}
