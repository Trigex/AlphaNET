using AlphaFramework.JS;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaFramework.Proxies
{
    class JSInterpreterProxy
    {
        private JSInterpreter _interpreter;

        public JSInterpreterProxy(JSInterpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public void ExecuteScript(string script, bool isTypescript)
        {
            _interpreter.ExecuteScript(script, isTypescript);
        }
    }
}
