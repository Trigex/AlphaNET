using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;

namespace AlphaNET.Framework.IO
{
    public class Filesystem
    {
        public List<FilesystemObject> FilesystemObjects { get; }
        public string FsPath;

        public Filesystem(string fsPath)
        {
            FsPath = fsPath;
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
                    // Recurrsion!!! This'll probably work, I hope
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
            BinaryManager.InsertFilesystemObjectIntoBinary(obj, FsPath, this);
        }
    }
}
