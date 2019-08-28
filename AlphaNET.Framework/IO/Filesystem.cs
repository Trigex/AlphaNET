using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace AlphaNET.Framework.IO
{
    /*
    public class Filesystem
    {
        public List<FilesystemObject> FilesystemObjects { get; }
        private readonly string _fsPath;
        private const int MemoryCap = 536870912; // 512 mb file contents memory cap

        public Filesystem(string fsPath)
        {
            _fsPath = fsPath;
            FilesystemObjects = new List<FilesystemObject>();
        }

        public IoStatusCode AddObject(FilesystemObject obj, Directory dir)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            // Check if the directory the object is being added to exists
            if (GetObjectById(dir.Id) == null)
            {
                return IoStatusCode.ObjectNotFound;
            }

            // Add to directory children
            dir.Children.Add(obj);
            obj.Owner = dir;
            // Add to FilesystemObjects list
            FilesystemObjects.Add(obj);

            // Append to bin
            WriteObject(obj);

            //BinaryManager.PrintFilesystem(BinaryManager.CreateBinaryFromFilesystem(this));

            return IoStatusCode.ObjectAdded;
        }


        public IoStatusCode AddObject(FilesystemObject obj)
        {
            // Add to FilesystemObjects list
            FilesystemObjects.Add(obj);
            WriteObject(obj);
            return IoStatusCode.ObjectAdded;
        }

        public IoStatusCode MoveObject(FilesystemObject obj, Directory dir)
        {
            // Check if the new directory's children list already has an object of obj's ID
            if (dir.GetChildById(obj.Id) != null)
            {
                return IoStatusCode.ObjectAlreadyPresent;
            }

            // Set FilesystemObject's Owner to the new directory
            obj.Owner = dir;
            // Add FilesystemObject to new directory's children
            dir.Children.Add(obj);
            return IoStatusCode.ObjectMoved;
        }

        public IoStatusCode DeleteObject(FilesystemObject obj)
        {
            // Check if FilesystemObject is actually present in the Filesystem
            if (GetObjectById(obj.Id) == null)
            {
                return IoStatusCode.ObjectNotFound;
            }

            // If it's a directory, delete all it's children
            if (obj.GetType() == typeof(Directory))
            {
                // Cast to Directory
                var dirObj = (Directory)obj;

                // Loop for all children
                foreach (var subObj in dirObj.Children.ToList())
                {
                    // Recursion!!! This'll probably work, I hope
                    DeleteObject(subObj);
                }
            }

            // Remove from Owner's object list
            obj.Owner.RemoveChildById(obj.Id);
            // Remove from Filesystem object list
            FilesystemObjects.Remove(GetObjectById(obj.Id));

            return IoStatusCode.ObjectDeleted;
        }

        public FilesystemObject GetObjectById(uint id)
        {
            return FilesystemObjects.FirstOrDefault(obj => obj.Id == id);
        }

        public List<FilesystemObject> GetObjectsByTitle(string title)
        {
            return FilesystemObjects.Where(obj => obj.Title == title).ToList();
        }

        public FilesystemObject GetObjectByTitle(string title)
        {
            return FilesystemObjects.FirstOrDefault(obj => obj.Title == title);
        }

        /// <summary>
        /// Return a <c>FilesystemObject</c> resource at the given absolute path
        /// </summary>
        /// <param name="path">Absolute path locating the <c>FilesystemObject</c></param>
        /// <returns><c>FilesystemObject</c> found at the absolute path</returns>
        public FilesystemObject GetObjectByAbsolutePath(string path)
        {
            // /bin/ls.js
            FilesystemObject returnObject = null;
            var resources = path.Split('/');
            var resourceList = new List<string>(resources);
            // Strip empty entries
            foreach (var resource in resources)
            {
                if (IsNullOrWhiteSpace(resource) || IsNullOrEmpty(resource))
                {
                    resourceList.Remove(resource);
                }
            }
            resources = resourceList.ToArray();

            // it's an absolute path, so start at root
            var dir = (Directory)GetObjectByTitle("root");
            foreach (var resource in resources)
            {
                var child = dir.GetChildByTitle(resource);

                if (child == null) continue;
                // Are we on the final resource?
                if (resources[resources.Length - 1] == resource)
                {
                    returnObject = child;
                }

                if (child.GetType() == typeof(Directory))
                {
                    dir = (Directory)child;
                }
            }

            return returnObject;
        }

        /// <summary>
        /// Return an absolute path to the given <c>FilesystemObject</c>
        /// </summary>
        /// <param name="fsObj"><c>FilesystemObject</c> to build the absolute path to</param>
        /// <returns>Absolute path to the <c>FilesystemObject</c></returns>
        public string GetAbsolutePathByObject(FilesystemObject fsObj)
        {
            var returnPath = "";
            // traverse fsObj parents until root is hit
            var resources = new List<string> {fsObj.Title};

            var start = fsObj.Owner;
            while (start.Title != "root")
            {
                resources.Add(start.Owner.Title);
                start = start.Owner;
            }

            // reverse resources list
            resources.Reverse();
            foreach (var resource in resources)
            {
                if (resource == "root")
                {
                    returnPath += "/";
                }

                // If the fsObj is a file, and we're on the last resource, don't append a "/"
                else if (fsObj.GetType() == typeof(File) && resources.ToArray()[resources.Count - 1] == resource)
                {
                    returnPath += resource;
                }
                else
                {
                    returnPath += resource + "/";
                }
            }

            return returnPath;
        }

        public Directory[] GetAllDirectories()
        {
            return FilesystemObjects.OfType<Directory>().ToArray();
        }

        public File[] GetAllFiles()
        {
            return FilesystemObjects.OfType<File>().ToArray();
        }

        public uint GenerateFilesystemObjectId()
        {
            var id = IOUtils.GenerateId();
            // Check for ID collision
            var objList = FilesystemObjects.Where(obj => obj.Id == id);
            if (objList.Any())
            {
                // generate another
                id = GenerateFilesystemObjectId();
            }

            return id;
        }

        private void WriteObject(FilesystemObject obj)
        {
            BinaryManager.InsertFilesystemObjectIntoBinary(obj, _fsPath, this);
        }
    }*/
    
    /// <summary>
    /// The Filesystem class interacts with a .fs Filesystem file, retrieving, and writing data representing the Filesystem to and from the file.
    /// </summary>
    public class Filesystem
    {
        #region Dynamic Properties
        /// <summary>
        /// The path to the .fs file this Filesystem instance is working with
        /// </summary>
        private readonly string _fsPath;
        /// <summary>
        /// The total size (in bytes) of the .fs file to which this Filesystem instance is assigned
        /// </summary>
        private int _fsSize;
        /// <summary>
        /// The total count of blocks present in the .fs file
        /// </summary>
        private int _blockCount;
        /// <summary>
        /// The total count of INodes present in the .fs file
        /// </summary>
        private int _inodeCount;
        /// <summary>
        /// The FileStream this instance will constantly have open, for interaction with the .fs file
        /// </summary>
        private readonly FileStream _fsStream;
        #endregion
        
        #region Constant Properties
        /// <summary>
        /// The size (in bytes) of a SuperBlock .fs file section
        /// </summary>
        private const int SuperBlockSectionSize = 37;
        /// <summary>
        /// The target .fs file version
        /// </summary>
        private const byte FsVersion = 1;
        /// <summary>
        /// The size (in bytes) of a single block in the .fs file
        /// </summary>
        private const int BlockSize = 4096;
        /// <summary>
        /// The size (in bytes) of a single INode object in the .fs file
        /// </summary>
        private const int INodeSize = 132;
        /// <summary>
        /// The INode Number assigned to the Filesystem's root Directory
        /// </summary>
        private const int RootINodeNumber = 1;
        
        private const int INodePerByteRatio = 16384;
        #endregion
        
        /// <summary>
        /// Instantiates a new Filesystem instance, and opens a FileStream
        /// </summary>
        /// <param name="fsPath">The path to the .fs file to open a FileStream from</param>
        public Filesystem(string fsPath)
        {
            _fsPath = fsPath;
            _fsStream = new FileStream(_fsPath, FileMode.OpenOrCreate);
        }

        ~Filesystem()
        {
            // Dispose of the FileStream when the Filesystem gets garbage collected
            _fsStream.Dispose();
        }
        
        /// <summary>
        /// Initializes the Filesystem's .fs file
        /// </summary>
        /// <param name="fsSize">The size (in megabytes) of the Filesystem file</param>
        public async Task InitializeFilesystem(int fsSize)
        {
            // Fill file with zeroes in worker thread
            await Task.Run(() =>
            {
                // Seeks to the last byte of the specified size
                _fsStream.Seek(fsSize, SeekOrigin.Begin);
                // Writes a zero, which would actually fill the file
                _fsStream.WriteByte(0);
                // set stream position back to 0
                ResetStreamPosition();
            });
            
            // set fs properties
            _fsSize = fsSize;
            _blockCount = fsSize / BlockSize;
            _inodeCount = fsSize / INodePerByteRatio;

            // Write Superblock
        }

        public void LoadFilesystem()
        {
            
        }

        private void ResetStreamPosition()
        {
            _fsStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
