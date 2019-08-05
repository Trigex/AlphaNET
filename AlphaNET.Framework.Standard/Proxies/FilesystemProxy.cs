using AlphaNET.Framework.Standard.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Standard.Proxies
{
    public class FilesystemProxy
    {
        public Directory Root;
        private Filesystem _filesystem;

        public FilesystemProxy(Filesystem filesystem)
        {
            _filesystem = filesystem;
            Root = (Directory)_filesystem.GetFilesystemObjectsByTitle("root")[0];
        }

        public File GetFileByTitle(string title)
        {
            var files = _filesystem.GetFilesystemObjectsByTitle(title);
            if(files != null && files.Length > 0)
            {
                if (files[0].GetType() == typeof(File))
                    return (File)files[0];
                else
                    return null;
            } else
            {
                return null;
            }
        }

        public Directory GetDirectoryByTitle(string title)
        {
            var dirs = _filesystem.GetFilesystemObjectsByTitle(title);
            if (dirs != null && dirs.Length > 0)
            {
                if (dirs[0].GetType() == typeof(Directory))
                    return (Directory)dirs[0];
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public FilesystemObject GetFilesystemObjectByAbsolutePath(string path)
        {
            return _filesystem.GetFilesystemObjectByAbsolutePath(path);
        }

        public string GetAbsolutePathByFilesystemObject(FilesystemObject fsObj)
        {
            return _filesystem.GetAbsolutePathByFilesystemObject(fsObj);
        }
    }
}
