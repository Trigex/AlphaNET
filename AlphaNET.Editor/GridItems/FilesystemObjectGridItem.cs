using AlphaNET.Framework.IO;
using Eto.Forms;

namespace AlphaNET.Editor.GridItems
{
    public class FilesystemObjectGridItem : TreeGridItem
    {
        public FilesystemObject FilesystemObject { get; set; }
        public uint ID { get; set; }
        public string Title { get; set; }
        public int Size { get; set; }
        public FilesystemObjectGridItem(uint id, string title, FilesystemObject fsObj = null) : base()
        {
            if (fsObj != null)
            {
                FilesystemObject = fsObj;
            }

            ID = id;
            Title = title;
            Tag = id;
        }
    }
}
