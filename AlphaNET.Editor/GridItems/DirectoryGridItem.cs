using AlphaNET.Framework.IO;

namespace AlphaNET.Editor.GridItems
{
    class DirectoryGridItem : FilesystemObjectGridItem
    {
        public DirectoryGridItem(uint id, string title, Directory fsObj = null) : base(id, title, fsObj)
        {
            if (fsObj != null)
            {
                Size = GetDirectorySize(fsObj, 0);
            }
        }

        public int GetDirectorySize(Directory dir, int size)
        {
            foreach (var child in dir.Children)
            {
                if (child.GetType() == typeof(File))
                {
                    var file = (File)child;
                    size += file.Contents.Length;
                }

                if (child.GetType() != typeof(Directory)) continue;
                var childDir = (Directory)child;
                GetDirectorySize(childDir, size);
            }

            return size;
        }
    }
}
