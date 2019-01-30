using System;
using System.IO;
using AlphaNET.IO;
using AlphaNET.Lua;
using AlphaNET.Utils;

namespace AlphaNET.Core
{
    class Computer
    {
        private Filesystem filesystem;
        private LuaManager luaManager;

        public Computer(string filesystemPath, string wsServerHost = null)
        {
            // create filesystem
            filesystem = new Filesystem(filesystemPath);
            filesystem.SaveFileSystemToJson();
            // create lua manager
            luaManager = new LuaManager(filesystem, this);
            InstallLuaScripts();
            // run init script
            luaManager.ExecuteScript(filesystem.GetFileByTitle("init.lua").data);
        }

        private void InstallLuaScripts()
        {
            // convert and install lua programs
            string[] luaFiles = System.IO.Directory.GetFiles(@"LuaScripts\");
            foreach (string filename in luaFiles)
            {
                // get text from file
                string luaCode = System.IO.File.ReadAllText(filename);
                // get the file in the virtual fs representing the real lua file
                IO.File vLuaFile = filesystem.GetFileByTitle(filename.Replace(@"LuaScripts\", String.Empty));
                // set that file's data to the code from the real file
                vLuaFile.data = luaCode;
            }
            // save the changes to the filesystem
            filesystem.SaveFileSystemToJson();
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
