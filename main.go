package main

import (
	"github.com/trigex/alphanet/core"
	"github.com/trigex/alphanet/io"
	"github.com/trigex/alphanet/js"
)

func main() {
	// create fs
	fs := io.CreateFilesystem("fs.json")
	// create modifications
	dir := io.CreateDirectory("usr", fs.Directories[0])
	file := io.CreateFile("zoomers.txt", "lol funny 4chan :)", &dir)
	fs.AddDirectory(dir)
	fs.AddFile(file)

	fs.DebugPrint()
	fs.Save("fs.json")

	// create javascript vm
	jsVm := js.CreateJsVm()
	jsVm.RunScript(`console.log("hello, world! ( from javascript :) )")`)

	// create computer, which holds these structs
	comp := core.CreateComputer(fs, jsVm)
	// start main loop
	comp.Start()
}
