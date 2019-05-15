package main

import (
	"github.com/trigex/alphanet/core"
	"github.com/trigex/alphanet/io"
	"github.com/trigex/alphanet/js"
)

func main() {
	// create fs
	fs := io.CreateFilesystem("fs.json")
	fs.DebugPrint()

	// create javascript vm
	jsVm := js.CreateJsVm()

	// create computer, which holds these structs
	comp := core.CreateComputer(fs, jsVm)
	// start main loop
	comp.Start()
}
