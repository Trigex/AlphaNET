package main

import (
	"fmt"

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
	jsVm.RunScript(`console.log("hello, world! ( from javascript :) )")`)

	// create computer, which holds these objects
	comp := core.CreateComputer(fs, jsVm)
	// start main loop
	comp.Start()
}

func PrintFs(fs io.Filesystem) {
	fmt.Println("Fs:")
	for i := range fs.Directories {
		PrintDirectory(*fs.Directories[i])
	}
}

func PrintDirectory(dir io.Directory) {
	fmt.Println("Directory: " + dir.Name + ", Parent: " + dir.ParentDirectory.Name)
	fmt.Println("Children: ")
	for d := range dir.ChildrenDirectories {
		fmt.Println("    Child Directory: " + dir.ChildrenDirectories[d].Name)
	}

	for f := range dir.ChildrenFiles {
		PrintFile(*dir.ChildrenFiles[f])
	}

	fmt.Println()
}

func PrintFile(file io.File) {
	fmt.Println("    Child File: " + file.Name + ", Parent: " + file.ParentDirectory.Name + ", Contents: " + file.Contents)
}
