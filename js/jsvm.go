package js

import (
	"bufio"
	"fmt"
	"os"
	"strings"

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
	vm := JsVm{*otto.New()}
	// set api functions
	vm.vm.Set("print", func(call otto.FunctionCall) otto.Value {
		fmt.Print(call.Argument(0).ToString())
		return otto.Value{}
	})

	vm.vm.Set("printLn", func(call otto.FunctionCall) otto.Value {
		fmt.Println(call.Argument(0).ToString())
		return otto.Value{}
	})

	vm.vm.Set("getLine", func(call otto.FunctionCall) otto.Value {
		reader := bufio.NewReader(os.Stdin)
		text, _ := reader.ReadString('\n')
		text = strings.Replace(text, "\n", "", -1)
		result, _ := otto.ToValue(text)
		return result
	})

	return vm
}
