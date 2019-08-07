using AlphaNET.Framework.IO;

namespace AlphaNET.Editor.GridItems
{
    public class FileGridItem : FilesystemObjectGridItem
    {
        public bool IsPlaintext { get; set; }
        public FileGridItem(uint id, string title, bool isPlaintext, File fsObj = null) : base(id, title, fsObj)
        {
            IsPlaintext = isPlaintext;
            if (fsObj != null)
            {
                Size = fsObj.Contents.Length;
            }
        }
    }
}
