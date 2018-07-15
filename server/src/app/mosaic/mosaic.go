package mosaic

import (
	"app/util"
	"app/appinfo"
	"path/filepath"
)

type Mosaic struct {
}

func (mos *Mosaic) Load() {
	path := filepath.Join(appinfo.Datapath, "constants.csv")
	util.LoadCsvTranspose(path, &Const)
}

func New() *Mosaic {
	return &Mosaic{}
}
