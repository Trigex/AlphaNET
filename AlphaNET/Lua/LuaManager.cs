using System;
using MoonSharp.Interpreter;
using AlphaNET.IO;
using AlphaNET.Core;

namespace AlphaNET.Lua
{
    class LuaManager
    {
        private Script script;
        private Computer computer;
        private Filesystem filesystem;

        public LuaManager(Filesystem filesystem, Computer computer)
        {
            script = new Script();
            this.filesystem = filesystem;
            this.computer = computer;
            // registers moonsharp user data
            UserData.RegisterAssembly();
            AddAPIs();
        }

        private void AddAPIs()
        {
            // Computer
            script.Globals["Computer"] = new ComputerProxy(computer);
            // LuaManager
            script.Globals["ExecuteScript"] = (Func<string, DynValue>)ExecuteScript;
            // FilesystemProxy instance
            script.Globals["Filesystem"] = new FilesystemProxy(filesystem);
            // Utils
            script.Globals["StringMagick"] = typeof(StringMagickProxy);
        }

        public DynValue ExecuteScript(string code)
        {
            DynValue res = script.DoString(code);
            return res;
        }
    }
}

