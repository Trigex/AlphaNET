﻿using AlphaNET.Framework.JS;
using System.Threading.Tasks;

namespace AlphaNET.Framework.Proxies
{
    class JSInterpreterProxy
    {
        private JSInterpreter _interpreter;

        public JSInterpreterProxy(JSInterpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public void Execute(string script, string[] args, bool blocking)
        {
            _interpreter.Execute(new Process(script, args), blocking);
        }

        public void Execute(Process process, bool blocking)
        {
            _interpreter.Execute(process, blocking);
        }
    }
}
