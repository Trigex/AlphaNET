namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// <c>File</c> is a class representing a <c>Filesystem</c> file; it is capable of holding arbitrary byte array contents
    /// </summary>
    public class File : FilesystemObject
    {
        /// <summary>
        /// Is <c>File.Contents</c> a UTF8 encoded string?
        /// </summary>
        public bool IsPlaintext { get; set; }
        /// <summary>
        /// Arbitrary byte array the <c>File</c> holds
        /// </summary>
        public byte[] Contents { get; set; }

        /// <summary>
        /// Instantiates a new File
        /// </summary>
        /// <param name="title">The title of the <c>File</c></param>
        /// <param name="owner">The <c>Directory</c> whom contains this <c>File</c> as a child</param>
        /// <param name="id">The ID of the <c>File</c></param>
        /// <param name="isPlaintext">Is the <c>contents</c> argument a UTF8 encoded string?</param>
        /// <param name="contents">Arbitrary byte array for the <c>File</c> to hold</param>
        public File(string title, Directory owner, uint id, bool isPlaintext, byte[] contents) : base(title, owner, id)
        {
            IsPlaintext = isPlaintext;
            Contents = contents;
        }

        /// <summary>
        /// Modify <c>File.Contents</c> with a new arbitrary byte array
        /// </summary>
        /// <param name="newContents">The new byte array to set</param>
        /// <param name="isPlaintext">Is the <c>newContents</c> argument a UTF8 encoded string?</param>
        /// <returns></returns>
        public StatusCode ModifyContents(byte[] newContents, bool isPlaintext)
        {
            IsPlaintext = isPlaintext;
            Contents = newContents;
            return StatusCode.FileModified;
        }
    }
}
