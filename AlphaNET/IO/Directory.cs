using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;

namespace AlphaNET.IO
{
    [MoonSharpUserData]
    class Directory
    {
        /// <summary>
        /// Title of the directory
        /// </summary>
        public string title { get; }
        /// <summary>
        /// Directories that are direct children of this directory
        /// </summary>
        public List<Directory> childrenDirectories { get; }
        /// <summary>
        /// Files that are direct children of this directory
        /// </summary>
        public List<File> childrenFiles { get; }
        /// <summary>
        /// The parent directory of this directory
        /// </summary>
        public Directory parentDirectory { get; set; }
        /// <summary>
        /// The directory's depth in the JSON representation of the filesystem
        /// </summary>
        public int depth { get; }

        /// <summary>
        /// Constructs the directory
        /// </summary>
        /// <param name="title">Title of the directory</param>
        /// <param name="depth">Directory's depth in the JSON representation of the filesystem</param>
        public Directory(string title, int depth)
        {
            this.title = title;
            childrenDirectories = new List<Directory>();
            childrenFiles = new List<File>();
            parentDirectory = null;
            this.depth = depth;
        }

        /// <summary>
        /// Add a file to this directory, and set the file's location to this directory
        /// </summary>
        /// <param name="file"></param>
        public void AddFile(File file)
        {
            childrenFiles.Add(file);
            file.parentDirectory = this;
        }

        /// <summary>
        /// Returns the newest file contained in the directory by name
        /// </summary>
        /// <param name="title">The file's title</param>
        /// <returns></returns>
        public File GetFile(string title)
        {
            return childrenFiles.FindLast(file => file.title == title);
        }

        /// <summary>
        /// Returns a list of all files in the directory
        /// </summary>
        /// <returns></returns>
        public List<File> GetAllFiles()
        {
            return childrenFiles;
        }

        public List<Directory> GetAllDirectories()
        {
            return childrenDirectories;
        }

        /// <summary>
        /// Add a directory to this directory, and set the directory's parent to this directory
        /// </summary>
        /// <param name="directory"></param>
        public void AddDirectory(Directory directory)
        {
            childrenDirectories.Add(directory);
            directory.parentDirectory = this;
        }

        /// <summary>
        /// Returns the newest added file contained in this directory
        /// </summary>
        /// <returns></returns>
        public File GetNewestAddedFile()
        {
            return childrenFiles[childrenFiles.Count - 1];
        }
    }
}
