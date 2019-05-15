package core

import (
	"fmt"
	"io/ioutil"

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
	comp := Computer{&fs, &jsVm, false}
	InstallScripts(&comp)

	return comp
}

func InstallScripts(comp *Computer) {
	// install scripts (won't be in the release)

	bin, err := io.FindDirectory("bin", *comp.fs)
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

		fsFile, err := io.FindFile(f.Name(), *comp.fs)
		if err != nil { // create file
			newFile := io.CreateFile(f.Name(), string(script), bin)
			comp.fs.AddFile(newFile)
		} else { // update file
			fsFile.Contents = string(script)
		}
	}
}
