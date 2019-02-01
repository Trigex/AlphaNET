using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace AlphaNET.IO
{
    /// <summary>
    /// Represents the computer's filesystem, used to access and write
    /// data to and from
    /// </summary>
    
    class Filesystem
    {
        /// <summary>
        /// List of all directories present in the filesystem
        /// </summary>
        public List<Directory> directories { get; }
        /// <summary>
        /// List of all files present in the filesystem
        /// </summary>
        public List<File> files { get; }
        /// <summary>
        /// The root directory of the filesystem
        /// </summary>
        public Directory rootDirectory { get; set; }
        /// <summary>
        /// The path used to read and write the JSON filesystem
        /// </summary>
        private string fsJsonPath;

        /// <summary>
        /// Constructs the filesystem object, and loads from a given JSON path
        /// </summary>
        /// <param name="fsJsonPath">Path to the JSON used to read and write from</param>
        public Filesystem(string fsJsonPath)
        {
            // init the motherfucker
            directories = new List<Directory>();
            files = new List<File>();
            this.fsJsonPath = fsJsonPath;
            // load the filesystem
            LoadFileSystemFromJson();
        }

        /// <summary>
        /// Create File and Directory objects used to represent the filesystem from the JSON path
        /// </summary>
        private void LoadFileSystemFromJson()
        {
            // get json string
            string fsJson = System.IO.File.ReadAllText(fsJsonPath);
            // allows us to read through json one line at a time
            JsonTextReader reader = new JsonTextReader(new StringReader(fsJson));
            // read through the json one line at a time
            while (reader.Read())
            {
                // Does value have data present?
                if (reader.Value != null)
                {
                    switch (reader.TokenType)
                    {
                        // value is directory or file
                        case JsonToken.PropertyName:
                            string val = (string)reader.Value;

                            // is directory
                            if (val.StartsWith("/"))
                            {
                                if (val == "/root") // root directory
                                {
                                    directories.Add(new Directory("root", reader.Depth));
                                    rootDirectory = GetLatestDirectory();
                                }
                                else // non root directory
                                {
                                    // remove the slash from the directory name
                                    string dirName = val.Trim('/');
                                    // create the new directory
                                    Directory newDir = new Directory(dirName, reader.Depth);
                                    if (reader.Depth == 2) // direct child of root
                                    {
                                        // add the directory as a child of root
                                        rootDirectory.AddDirectory(newDir);
                                    }
                                    else if (reader.Depth > 2) // outside of immediate root children
                                    {
                                        GetDirectoryByDepth(reader.Depth - 1).AddDirectory(newDir);
                                    }
                                    // add the directory to the master directory list
                                    directories.Add(newDir);
                                }

                            }
                            else // is file
                            {
                                File newFile = new File(val, null);

                                if (reader.Depth == 2)
                                {
                                    rootDirectory.AddFile(newFile);
                                }
                                else if (reader.Depth > 2)
                                {
                                    GetDirectoryByDepth(reader.Depth - 1).AddFile(newFile);
                                    
                                }
                                files.Add(newFile);
                            }
                            break;
                        // value is file data
                        case JsonToken.String:
                            string fileVal = (string)reader.Value;
                            // set the last file's data to this value
                            GetLatestFile().data = fileVal;
                            break;
                    }
                }
            }
            reader.Close();
        }

        /// <summary>
        /// Save the current JSON string representation of the filesystem to fsJsonPath
        /// </summary>
        public void SaveFileSystemToJson()
        {
            System.IO.File.WriteAllText(fsJsonPath, FilesystemToJson());
            // reload the filesystem from json
            //LoadFileSystemFromJson();
        }

        /// <summary>
        /// Create JSON string representation of the filesystemm
        /// </summary>
        /// <returns></returns>
        public string FilesystemToJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();
                writer.WritePropertyName("/root");
                writer.WriteStartObject();
                // now, let's get le recursive party started
                WalkDirectoriesAndWrite(rootDirectory, writer);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }
            string jsonText = sb.ToString();
            sw.Close();

            return jsonText;
        }

        public void ReloadFilesystem()
        {
            directories.Clear();
            files.Clear();
            rootDirectory = null;
            LoadFileSystemFromJson();
        }

        /// <summary>
        /// I honestly have no fucking clue how I managed this, but recursion is my new best friend
        /// </summary>
        /// <param name="dir">The starting directory</param>
        /// <param name="writer">The JSON writer that's being used to log the trip</param>
        private void WalkDirectoriesAndWrite(Directory dir, JsonWriter writer)
        {
            var files = dir.childrenFiles;
            foreach (var file in files)
            {
                writer.WritePropertyName(file.title);
                writer.WriteValue(file.data);
            }

            var subDirs = dir.childrenDirectories;
            foreach (var subDir in subDirs)
            {
                writer.WritePropertyName("/" + subDir.title);
                writer.WriteStartObject();
                WalkDirectoriesAndWrite(subDir, writer);
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Returns the newest directory in directories
        /// </summary>
        /// <returns></returns>
        public Directory GetLatestDirectory()
        {
            return directories[directories.Count - 1];
        }

        /// <summary>
        /// Returns the newest file in files
        /// </summary>
        /// <returns></returns>
        public File GetLatestFile()
        {
            return files[files.Count - 1];
        }

        /// <summary>
        /// Returns the newest directory with the given depth
        /// </summary>
        /// <param name="depth">The depth to look for</param>
        /// <returns></returns>
        private Directory GetDirectoryByDepth(int depth)
        {
            return directories.FindLast(dir => dir.depth == depth);
        }

        /// <summary>
        /// Returns the newest directory with the given title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public Directory GetDirectoryByTitle(string title)
        {
            return directories.FindLast(dir => dir.title == title);
        }

        /// <summary>
        /// Returns the newest file with the given title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public File GetFileByTitle(string title)
        {
            return files.FindLast(file => file.title == title);
        }
    }
}
