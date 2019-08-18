namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// <c>FilesystemObject</c> is an abstract class representing a given Filesystem entity (<c>File</c> and <c>Directory</c>)
    /// </summary>
    public abstract class FilesystemObject
    {
        /// <summary>
        /// The title of this <c>FilesystemObject</c>
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// The <c>Directory</c> owner of this <c>FilesystemObject</c>
        /// </summary>
        public Directory Owner { get; set; }
        /// <summary>
        /// The ID of this <c>FilesystemObject</c>
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// Base constructor for classes extending this class to call
        /// </summary>
        /// <param name="title">The title of the <c>FilesystemObject</c></param>
        /// <param name="owner">The <c>Directory</c> owner of the <c>FilesystemObject</c></param>
        /// <param name="id">The ID of the <c>FilesystemObject</c></param>
        public FilesystemObject(string title, Directory owner, uint id)
        {
            Title = title;
            Owner = owner;
            ID = id;
        }

        /// <summary>
        /// Modify the name of this <c>FilesystemObject</c>
        /// </summary>
        /// <param name="title">The title to rename this <c>FilesystemObject</c> to</param>
        /// <returns>A <c>IOStatusCode</c> representing the success of this renaming operation</returns>
        public IOStatusCode Rename(string title)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title))
            {
                return IOStatusCode.ObjectNotRenamed;
            }

            Title = title;
            return IOStatusCode.ObjectRenamed;
        }
    }
}
