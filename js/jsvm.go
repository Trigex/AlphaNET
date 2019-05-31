package js

import (
	"fmt"

	"github.com/robertkrimen/otto"
	_ "github.com/robertkrimen/otto/underscore"
	"github.com/trigex/alphanet/io"
)

// VM is a struct holding pointers to the various objects it requires
type VM struct {
	vm otto.Otto
	// pointers to structs the vm uses in it's api
	// these should probably the the same pointers the computer holds
	console *io.Console
	fs      *io.Filesystem
}

// RunScript runs a given string on the VM, printing an error if one appears
func (vm *VM) RunScript(script string) {
	// compile script before hand
	scr, cErr := vm.vm.Compile("", script)

	if cErr != nil {
		fmt.Printf("Error!: %s", cErr.Error())
	} else {
		// run script
		result, rErr := vm.vm.Run(scr)

		if rErr != nil {
			resultS, _ := result.ToString()
			fmt.Printf("Error!: %s, Result: %s", rErr.Error(), resultS)
		}
	}
}

// InstallAPIs setups a various AlphaNET APIs used by the VM
func (vm *VM) InstallAPIs() {
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

// CreateVM returns a new VM object
func CreateVM(c *io.Console, fs *io.Filesystem) VM {
	vm := VM{*otto.New(), c, fs}
	vm.InstallAPIs()
	return vm
}
