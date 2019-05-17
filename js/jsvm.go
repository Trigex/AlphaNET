package js

import (
	"fmt"

	"github.com/robertkrimen/otto"
	_ "github.com/robertkrimen/otto/underscore"
	"github.com/trigex/alphanet/io"
)

type JsVM struct {
	vm otto.Otto
	// pointers to structs the vm uses in it's api
	// these should probably the the same pointers the computer holds
	console *io.Console
	fs      *io.Filesystem
}

func (vm *JsVM) RunScript(script string) {
	result, err := vm.vm.Run(script)

	if err != nil {
		resultS, _ := result.ToString()
		fmt.Printf("Error!: %s, Result: %s", err.Error(), resultS)
	}
}

func (vm *JsVM) InstallAPIs() {
	c := vm.console
	fs := vm.fs

	// Console
	vm.vm.Set("print", func(call otto.FunctionCall) otto.Value {
		arg, _ := call.Argument(0).ToString()
		c.Print(arg)
		return otto.NullValue()
	})

	vm.vm.Set("printLn", func(call otto.FunctionCall) otto.Value {
		arg, _ := call.Argument(0).ToString()
		c.PrintLn(arg)
		return otto.NullValue()
	})

	vm.vm.Set("readLine", func(call otto.FunctionCall) otto.Value {
		out := c.ReadLine()
		val, _ := otto.ToValue(out)
		return val
	})

	// FIlesystem
	vm.vm.Set("getFile", func(call otto.FunctionCall) otto.Value {
		// this should later return a proper File struct
		// but for now just return the string of the file
		arg, _ := call.Argument(0).ToString()
		file, _ := fs.FindFile(arg)
		val, _ := otto.ToValue(file.Contents)
		return val
	})

	// VM
	vm.vm.Set("run", func(call otto.FunctionCall) otto.Value {
		arg, _ := call.Argument(0).ToString()
		result, _ := vm.vm.Run(arg)
		return result
	})
}

func CreateJsVM(c *io.Console, fs *io.Filesystem) JsVM {
	vm := JsVM{*otto.New(), c, fs}
	vm.InstallAPIs()
	return vm
}
