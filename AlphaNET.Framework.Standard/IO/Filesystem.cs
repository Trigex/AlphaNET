using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaNET.Framework.Standard.IO
{
    public class Filesystem
    {
        public List<FilesystemObject> FilesystemObjects { get; }
        public Filesystem()
        {
            FilesystemObjects = new List<FilesystemObject>();
        }

        public StatusCode AddFilesystemObject(FilesystemObject obj, Directory dir)
        {
            // Check if the directory the object is being added to exists
            if(GetFilesystemObjectByID(dir.ID) == null)
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

        public StatusCode AddFilesystemObject(FilesystemObject obj)
        {
            // Add to FilesystemObjects list
            FilesystemObjects.Add(obj);
            return StatusCode.ObjectAdded;
        }

        public StatusCode MoveFilesystemObject(FilesystemObject obj, Directory dir)
        {
            // Check if the new directory's children list already has an object of obj's ID
            if(dir.GetChildFilesystemObjectByID(obj.ID) != null)
            {
                return StatusCode.ObjectAlreadyPresent;
            }

            // Set FilesystemObject's Owner to the new directory
            obj.Owner = dir;
            // Add FilesystemObject to new directory's children
            dir.Children.Add(obj);
            return StatusCode.ObjectMoved;
        }

        public StatusCode DeleteFilesystemObject(FilesystemObject obj)
        {
            // Check if FilesystemObject is actually present in the Filesystem
            if(GetFilesystemObjectByID(obj.ID) == null)
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
                    DeleteFilesystemObject(subObj);
                }
            }

            // Remove from Owner's object list
            obj.Owner.RemoveChildFilesystemObjectByID(obj.ID);
            // Remove from Filesystem object list
            FilesystemObjects.Remove(GetFilesystemObjectByID(obj.ID));

            return StatusCode.ObjectDeleted;
        }

        public FilesystemObject GetFilesystemObjectByID(uint id)
        {
            var objects = FilesystemObjects.Where(obj => obj.ID == id).ToArray();

            if (objects == null || objects.Length == 0)
            {
                return null;
            }

            return objects[0];
        }

        public FilesystemObject[] GetFilesystemObjectsByTitle(string title)
        {
            var objects = FilesystemObjects.Where(obj => obj.Title == title).ToArray();
            if (objects == null || objects.Length == 0)
            {
                return null;
            }

            return objects;
        }

        /// <summary>
        /// Return a <c>FilesystemObject</c> resource at the given absolute path
        /// </summary>
        /// <param name="path">Absolute path locating the <c>FilesystemObject</c></param>
        /// <returns><c>FilesystemObject</c> found at the absolute path</returns>
        public FilesystemObject GetFilesystemObjectByAbsolutePath(string path)
        {
            // /bin/ls.js
            FilesystemObject returnObject = null;
            string[] resources = path.Split('/');
            var resourceList = new List<string>(resources);
            // Strip empty entries
            foreach(string resource in resources)
            {
                if (String.IsNullOrWhiteSpace(resource) || String.IsNullOrEmpty(resource))
                    resourceList.Remove(resource);
            }
            resources = resourceList.ToArray();

            // it's an absolute path, so start at root
            Directory dir = (Directory)GetFilesystemObjectsByTitle("root")[0];
            foreach(string resource in resources)
            {
                FilesystemObject child = null;
                Console.WriteLine(resource);
                child = dir.GetChildFilesystemObjectByTitle(resource);

                if(child != null)
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
        public string GetAbsolutePathByFilesystemObject(FilesystemObject fsObj)
        {
            string returnPath = "";
            // traverse fsObj parents until root is hit
            List<string> resources = new List<string>();
            resources.Add(fsObj.Title);

            Directory start = fsObj.Owner;
            while(start.Title != "root")
            {
                resources.Add(start.Owner.Title);
                start = start.Owner;
            }

            // reverse resources list
            resources.Reverse();
            foreach(string resource in resources)
            {
                if (resource == "root")
                    returnPath += "/";

                // If the fsObj is a file, and we're on the last resource, don't append a "/"
                else if(fsObj.GetType() == typeof(File) && resources.ToArray()[resources.Count-1] == resource)
                {
                    returnPath += resource;
                } else
                {
                    returnPath += resource + "/";
                }
            }

            Console.WriteLine(returnPath);

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
    }
}
