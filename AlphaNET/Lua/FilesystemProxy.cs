using MoonSharp.Interpreter;
using AlphaNET.IO;

namespace AlphaNET.Lua
{
    /// <summary>
    /// Proxy to the Filesystem class, used for saftey in Lua scripts
    /// </summary>
    [MoonSharpUserData]
    class FilesystemProxy
    {
        private Filesystem filesystem;
        public Directory rootDirectory;

        public FilesystemProxy(Filesystem filesystem)
        {
            this.filesystem = filesystem;
            rootDirectory = filesystem.rootDirectory;
        }

        public string GetFilesystemJson()
        {
            return filesystem.FilesystemToJson();
        }

        public void SaveFilesystem()
        {
            filesystem.SaveFileSystemToJson();
        }

        public Directory GetDirectoryByTitle(string title)
        {
            return filesystem.GetDirectoryByTitle(title);
        }

        public File GetFileByTitle(string title)
        {
            return filesystem.GetFileByTitle(title);
        }

        public Directory GetLatestDirectory()
        {
            return filesystem.GetLatestDirectory();
        }

        public File GetLatestFile()
        {
            return filesystem.GetLatestFile();
        }
    }
}
