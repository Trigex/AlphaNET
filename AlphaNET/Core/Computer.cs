using System;
using System.IO;
using AlphaNET.IO;
using AlphaNET.Lua;
using AlphaNET.Utils;

namespace AlphaNET.Core
{
    class Computer
    {
        public Filesystem filesystem;
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
                string name = filename.Replace(@"LuaScripts\", String.Empty);
                // get the file in the virtual fs representing the real lua file
                if (filesystem.GetFileByTitle(name) == null)
                {
                    filesystem.GetDirectoryByTitle("bin").AddFile(new IO.File(name, luaCode));
                } else
                {
                    filesystem.GetFileByTitle(name).data = luaCode;
                }
                
            }
            // save the changes to the filesystem
            filesystem.SaveFileSystemToJson();
            filesystem.ReloadFilesystem();
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
