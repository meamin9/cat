package app

import "testing"

func Test_LoadCfg(t *testing.T) {
	a := newApp()
	a.LoadCfg()
}