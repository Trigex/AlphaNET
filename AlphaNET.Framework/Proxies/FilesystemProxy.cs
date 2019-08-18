using AlphaNET.Framework.IO;

namespace AlphaNET.Framework.Proxies
{
    public class FilesystemProxy
    {
        public Directory Root;
        private Filesystem _filesystem;

        public FilesystemProxy(Filesystem filesystem)
        {
            _filesystem = filesystem;
            Root = (Directory)_filesystem.GetObjectsByTitle("root")[0];
        }

        public File GetFileByTitle(string title)
        {
            var files = _filesystem.GetObjectsByTitle(title);
            if (files != null && files.Count > 0)
            {
                if (files[0].GetType() == typeof(File))
                {
                    return (File)files[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Directory GetDirectoryByTitle(string title)
        {
            var dirs = _filesystem.GetObjectsByTitle(title);
            if (dirs != null && dirs.Count > 0)
            {
                if (dirs[0].GetType() == typeof(Directory))
                {
                    return (Directory)dirs[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public FilesystemObject GetObjectByAbsolutePath(string path)
        {
            return _filesystem.GetObjectByAbsolutePath(path);
        }

        public string GetAbsolutePathByObject(FilesystemObject fsObj)
        {
            return _filesystem.GetAbsolutePathByObject(fsObj);
        }

        public uint GenerateFilesystemObjectID()
        {
            return _filesystem.GenerateFilesystemObjectID();
        }

        public StatusCode AddObject(FilesystemObject fsObj, Directory dir)
        {
            return _filesystem.AddObject(fsObj, dir);
        }

        public StatusCode DeleteObject(FilesystemObject fsObj)
        {
            return _filesystem.DeleteObject(fsObj);
        }
    }
}
