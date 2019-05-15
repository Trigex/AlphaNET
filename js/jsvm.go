package js

import (
	"github.com/robertkrimen/otto"
	_ "github.com/robertkrimen/otto/underscore"
)

type JsVm struct {
	vm otto.Otto
}

func (vm *JsVm) RunScript(script string) {
	vm.vm.Run(script)
}

func CreateJsVm() JsVm {
	return JsVm{*otto.New()}
}
