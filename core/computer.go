package core

import (
	"github.com/trigex/alphanet/io"
	"github.com/trigex/alphanet/js"
)

type Computer struct {
	fs      *io.Filesystem
	jsVm    *js.JsVm
	running bool
}

func (comp *Computer) Start() {
	comp.running = true

	for comp.running {
		// run shell script
	}
}

func CreateComputer(fs io.Filesystem, jsVm js.JsVm) Computer {
	return Computer{&fs, &jsVm, false}
}
