using System;
using System.Collections.Generic;
using System.Linq;

namespace AlphaNET.Framework.IO
{
    public class Filesystem
    {
        public List<FilesystemObject> FilesystemObjects { get; }
        public Filesystem()
        {
            FilesystemObjects = new List<FilesystemObject>();
        }

        public StatusCode AddObject(FilesystemObject obj, Directory dir)
        {
            // Check if the directory the object is being added to exists
            if (GetObjectByID(dir.ID) == null)
            {
                return StatusCode.ObjectNotFound;
            }

            // Add to directory children
            dir.Children.Add(obj);
            obj.Owner = dir;
            // Add to FilesystemObjects list
            FilesystemObjects.Add(obj);

            return StatusCode.ObjectAdded;
        }


        public StatusCode AddObject(FilesystemObject obj)
        {
            // Add to FilesystemObjects list
            FilesystemObjects.Add(obj);
            return StatusCode.ObjectAdded;
        }

        public StatusCode MoveObject(FilesystemObject obj, Directory dir)
        {
            // Check if the new directory's children list already has an object of obj's ID
            if (dir.GetChildByID(obj.ID) != null)
            {
                return StatusCode.ObjectAlreadyPresent;
            }

            // Set FilesystemObject's Owner to the new directory
            obj.Owner = dir;
            // Add FilesystemObject to new directory's children
            dir.Children.Add(obj);
            return StatusCode.ObjectMoved;
        }

        public StatusCode DeleteObject(FilesystemObject obj)
        {
            // Check if FilesystemObject is actually present in the Filesystem
            if (GetObjectByID(obj.ID) == null)
            {
                return StatusCode.ObjectNotFound;
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
            obj.Owner.RemoveChildByID(obj.ID);
            // Remove from Filesystem object list
            FilesystemObjects.Remove(GetObjectByID(obj.ID));

            return StatusCode.ObjectDeleted;
        }

        public FilesystemObject GetObjectByID(uint id)
        {
            return FilesystemObjects.Where(obj => obj.ID == id).SingleOrDefault();
        }

        public List<FilesystemObject> GetObjectsByTitle(string title)
        {
            return FilesystemObjects.Where(obj => obj.Title == title).ToList();
        }

        public FilesystemObject GetObjectByTitle(string title)
        {
            return FilesystemObjects.Where(obj => obj.Title == title).SingleOrDefault();
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
            string[] resources = path.Split('/');
            var resourceList = new List<string>(resources);
            // Strip empty entries
            foreach (string resource in resources)
            {
                if (String.IsNullOrWhiteSpace(resource) || String.IsNullOrEmpty(resource))
                {
                    resourceList.Remove(resource);
                }
            }
            resources = resourceList.ToArray();

            // it's an absolute path, so start at root
            Directory dir = (Directory)GetObjectByTitle("root");
            foreach (string resource in resources)
            {
                FilesystemObject child = null;
                child = dir.GetChildByTitle(resource);

                if (child != null)
                {
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
            string returnPath = "";
            // traverse fsObj parents until root is hit
            List<string> resources = new List<string>();
            resources.Add(fsObj.Title);

            Directory start = fsObj.Owner;
            while (start.Title != "root")
            {
                resources.Add(start.Owner.Title);
                start = start.Owner;
            }

            // reverse resources list
            resources.Reverse();
            foreach (string resource in resources)
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

        public uint GenerateFilesystemObjectID()
        {
            var id = IOUtils.GenerateID();
            // Check for ID collision
            var objList = FilesystemObjects.Where(obj => obj.ID == id);
            if (objList.Count() > 0)
            {
                // generate another
                id = GenerateFilesystemObjectID();
            }

            return id;
        }
    }
}
