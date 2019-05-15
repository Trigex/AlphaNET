package core

import (
	"fmt"
	"io/ioutil"

	"github.com/trigex/alphanet/io"
	"github.com/trigex/alphanet/js"
)

type Computer struct {
	fs      *io.Filesystem
	jsVM    *js.JsVM
	running bool
}

func (comp *Computer) Start() {
	comp.running = true
	// get init script
	init, err := comp.fs.FindFile("init.js")
	if err != nil {
		//fmt.Println(err)
	}

	for comp.running {
		// run shell script
		comp.jsVM.RunScript(init.Contents)
	}
}

func CreateComputer(fs io.Filesystem, jsVM js.JsVM) Computer {
	comp := Computer{&fs, &jsVM, false}
	InstallScripts(&comp)
	return comp
}

func InstallScripts(comp *Computer) {
	// install scripts (won't be in the release)

	bin, err := comp.fs.FindDirectory("bin")
	if err != nil {
		fmt.Println(err)
	}

	// get scripts/ files
	files, err := ioutil.ReadDir("./scripts/")
	if err != nil {
		fmt.Println(err)
	}

	for _, f := range files {
		// read
		script, err := ioutil.ReadFile("./scripts/" + f.Name())
		if err != nil {
			fmt.Println(err)
		}

		fsFile, err := comp.fs.FindFile(f.Name())
		if err != nil { // create file
			newFile := io.CreateFile(f.Name(), string(script), bin)
			comp.fs.AddFile(newFile)
		} else { // update file
			fsFile.Contents = string(script)
		}
	}

	comp.fs.Save()
}
