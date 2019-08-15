using AlphaNET.Framework.JS;

namespace AlphaNET.Framework.Proxies
{
    class JSInterpreterProxy
    {
        private JSInterpreter _interpreter;

        public JSInterpreterProxy(JSInterpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public int ExecuteScript(string script, bool isTypescript, string[] args)
        {
            return _interpreter.ExecuteScript(script, isTypescript, args);
        }
    }
}
