using System.Collections.Generic;
using System.Linq;

namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// <c>Directory</c> is a class representing a <c>Filesystem</c> directory; it is capable of holding other objects extending <c>FilesystemObject</c>
    /// </summary>
    public class Directory : FilesystemObject
    {
        /// <summary>
        /// The child <c>FilesystemObject</c>s this <c>Directory</c> holds
        /// </summary>
        public List<FilesystemObject> Children { get; set; }

        /// <summary>
        /// Instantiates a new Directory
        /// </summary>
        /// <param name="title">The title of the <c>Directory</c></param>
        /// <param name="id">The ID of the <c>Directory</c></param>
        public Directory(string title, uint id) : base(title, null, id)
        {
            Children = new List<FilesystemObject>();
        }

        /// <summary>
        /// Returns the first child <c>FilesystemObject</c> with the given ID
        /// </summary>
        /// <param name="id">The ID to query the <c>Directory</c>'s children with</param>
        /// <returns>The first result of the query; null if no matching child was found</returns>
        public FilesystemObject GetChildByID(uint id)
        {
            if (Children.Count < 1)
            {
                return null;
            }

            return Children.Where(obj => obj.ID == id).ToArray()[0];
        }

        /// <summary>
        /// Removes the first child <c>FilesystemObject</c> with the given ID
        /// </summary>
        /// <param name="id">The ID to query the <c>Directory</c>'s children with</param>
        /// <returns>The <c>StatusCode</c> representing the success status of the operation</returns>
        public StatusCode RemoveChildByID(uint id)
        {
            // attempt to remove the object of the given ID
            if (Children.Remove(GetChildByID(id)))
            {
                return StatusCode.ObjectDeleted;
            }
            else
            {
                return StatusCode.ObjectNotDeleted;
            }
        }

        /// <summary>
        /// Returns the first child <c>FilesystemObject</c> with the given title
        /// </summary>
        /// <param name="title">The title to query the <c>Directory</c>'s children with</param>
        /// <returns>The first result of the query; null if no matching child was found</returns>
        public FilesystemObject GetChildByTitle(string title)
        {
            if (Children.Count < 1)
            {
                return null;
            }

            var objects = Children.Where(obj => obj.Title == title).ToArray();
            if (objects == null || objects.Length == 0)
            {
                return null;
            }

            return objects[0];
        }

        public List<FilesystemObject> GetChildrenByTitle(string title)
        {
            if (Children.Count < 1)
            {
                return null;
            }

            return Children.Where(obj => obj.Title == title).ToList();
        }

        public List<FilesystemObject> GetChildren()
        {
            return Children;
        }
    }
}
