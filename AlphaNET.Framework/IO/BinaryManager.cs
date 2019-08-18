using AlphaNET.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// <c>BinaryManager</c> is a static util class with methods enabling the reading, 
    /// and writing of a <c>Filesystem</c> to a binary format
    /// </summary>
    public static class BinaryManager
    {
        private const byte FsVersionTarget = 1;
        // Section flag magic numbers
        private const byte FsHeaderStart = 6;
        private const byte FsHeaderEnd = 9;
        private const byte DirListStart = 1;
        private const byte DirListEnd = 2;
        private const byte FileListStart = 3;
        private const byte FileListEnd = 4;
        private const byte DirStart = 5;
        private const byte DirEnd = 6;
        private const byte FileStart = 7;
        private const byte FileEnd = 8;

        /// <summary>
        /// Creates a binary encoded <c>Filesystem</c> from a <c>Filesystem</c> instance
        /// </summary>
        /// <param name="filesystem">The <c>Filesystem</c> to create the binary from</param>
        /// <returns>A byte array representing the binary encoded <c>Filesystem</c></returns>
        public static byte[] CreateBinaryFromFilesystem(Filesystem filesystem)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            var directories = new List<Directory>();
            var files = new List<File>();

            // Get files and directories
            foreach (var fsObj in filesystem.FilesystemObjects)
            {
                if (fsObj.GetType() == typeof(Directory))
                {
                    directories.Add((Directory)fsObj);
                }

                if (fsObj.GetType() == typeof(File))
                {
                    files.Add((File)fsObj);
                }
            }

            writer.Write(FsHeaderStart); // FS Header start flag
            writer.Write(FsVersionTarget); // FS Version
            writer.Write(FsHeaderEnd); // FS Header end flag

            writer.Write(DirListStart); // Dir list start flag
            foreach (var dir in directories)
            {
                writer.Write(DirStart); // dir start flag
                WriteGenericObjectMeta(writer, dir);
                writer.Write(DirEnd); // dir end flag
            }
            writer.Write(DirListEnd); // Dir list end flag

            writer.Write(FileListStart);
            foreach (var file in files)
            {
                writer.Write(FileStart); // file start flag
                WriteGenericObjectMeta(writer, file);
                writer.Write(file.IsPlaintext); // Plaintext?
                writer.Write((uint)file.Contents.Length); // Contents length
                writer.Write(file.Contents); // Contents
                writer.Write(FileEnd); // file end flag
            }
            writer.Write(FileListEnd);
            writer.Close();
            return stream.ToArray();
        }

        /// <summary>
        /// Creates a <c>Filesystem</c> instance from a binary encoded <c>Filesystem</c> byte array
        /// </summary>
        /// <param name="bin">The binary encoded <c>Filesystem</c> byte array</param>
        /// <returns>An instance of <c>Filesystem</c> representing the binary encoded <c>Filesystem</c></returns>
        public static Filesystem CreateFilesystemFromBinary(byte[] bin)
        {
            var stream = new MemoryStream(bin);
            var reader = new BinaryReader(stream);
            var fs = new Filesystem();

            var headerStart = reader.ReadByte();
            if (headerStart != FsHeaderStart) // FS File Header Start
            {
                throw new InvalidFilesystemHeaderException("Start", headerStart);
            }

            byte fsVersion = reader.ReadByte(); // FS Version

            var headerEnd = reader.ReadByte();
            if (headerEnd != FsHeaderEnd) // FS File Header End
            {
                throw new InvalidFilesystemHeaderException("End", headerEnd);
            }

            var dirListStart = reader.ReadByte();
            if (dirListStart != DirListStart)// Directory List Start
            {
                throw new InvalidFilesystemHeaderException("Directory List Start", dirListStart);
            }

            // TODO: Directory Exceptions

            ReadDirectories(reader, fs);

            // The ReadDirectories method consumes DIR_LIST_END, in order to know when to stop, so it's not present here
            var fileListStart = reader.ReadByte();
            if (fileListStart != FileListStart)// File List Start
            {
                throw new InvalidFilesystemHeaderException("File List Start", fileListStart);
            }

            ReadFiles(reader, fs);

            // The ReadFiles method consumes FILE_LIST_END, in order to know when to stop, so it's not present here
            // Filesystem should be complete!

            reader.Close();

            return fs;
        }

        /// <summary>
        /// Reloads a given <c>Filesystem</c> from the contents of a binary encoded <c>Filesystem</c>
        /// </summary>
        /// <param name="filesystem"><c>Filesystem</c> instance to be reloaded</param>
        /// <param name="bin">Binary encoded <c>Filesystem</c> byte array to load from</param>
        public static void ReloadFilesystemFromBinary(Filesystem filesystem, byte[] bin)
        {
            if (filesystem == null) throw new ArgumentNullException(nameof(filesystem));
            filesystem = CreateFilesystemFromBinary(bin);
        }

        /// <summary>
        /// Reads the binary contents of a file at the given path
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>Byte array of the file's contents</returns>
        public static byte[] ReadBinaryFromFile(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }

        /// <summary>
        /// Writes a byte array to a file at the given path
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="bin">Byte array to write to the file</param>
        public static void WriteBinaryToFile(string path, byte[] bin)
        {
            System.IO.File.WriteAllBytes(path, bin);
        }

        /// <summary>
        /// Writes generic FilesystemObject data to a given <c>BinaryWriter</c> based off the given <c>FilesystemObject</c>
        /// </summary>
        /// <param name="writer">The <c>BinaryWriter</c> to be used for writing</param>
        /// <param name="fsObj">The <c>FilesystemObject</c> to write the generic data from</param>
        private static void WriteGenericObjectMeta(BinaryWriter writer, FilesystemObject fsObj)
        {
            writer.Write(fsObj.Id); // ID
            writer.Write(fsObj.Owner.Id); // OwnerID
            writer.Write((ushort)Encoding.UTF8.GetByteCount(fsObj.Title)); // Title length
            writer.Write(Encoding.UTF8.GetBytes(fsObj.Title)); // title
        }

        /// <summary>
        /// Reads generic FilesystemObject data from a given <c>BinaryWriter</c>
        /// </summary>
        /// <param name="reader">The <c>BinaryReader</c> to read from</param>
        /// <returns></returns>
        private static GenericObjectMeta ReadGenericObjectMeta(BinaryReader reader)
        {
            var genericObjectMeta = new GenericObjectMeta
            {
                Id = reader.ReadUInt32(), OwnerId = reader.ReadUInt32(), TitleLength = reader.ReadUInt16()
            };

            genericObjectMeta.Title = reader.ReadBytes(genericObjectMeta.TitleLength);

            return genericObjectMeta;
        }

        /// <summary>
        /// Loops through the DirectoriesList of a binary encoded <c>Filesystem</c>, modifying the <c>Filesystem</c> instance procedurely based off what it reads
        /// </summary>
        /// <param name="reader">The <c>BinaryReader</c> to read from</param>
        /// <param name="filesystem">The <c>Filesystem</c> to modify based off what's read</param>
        private static void ReadDirectories(BinaryReader reader, Filesystem filesystem)
        {
            var listEnd = false;
            while (!listEnd)
            {
                var unused = reader.ReadByte();
                var dirMeta = ReadGenericObjectMeta(reader);
                var dirEnd = reader.ReadByte();

                var newDir = new Directory(Encoding.UTF8.GetString(dirMeta.Title), dirMeta.Id);
                if (newDir.Title == "root")
                {
                    newDir.Owner = newDir;
                    filesystem.AddObject(newDir);
                }
                else
                {
                    var dirOwner = (Directory)filesystem.GetObjectById(dirMeta.OwnerId);

                    if (dirOwner == null | dirOwner.GetType() != typeof(Directory))
                    {
                        // Error!
                    }

                    filesystem.AddObject(newDir, dirOwner);
                }

                if (reader.ReadByte() == DirListEnd)
                {
                    listEnd = true;
                }
                else
                {
                    reader.BaseStream.Position -= 1; // go back a byte
                }
            }
        }

        /// <summary>
        /// Loops through the FilesList of a binary encoded <c>Filesystem</c>, modifying the <c>Filesystem</c> instance procedurely based off what it reads
        /// </summary>
        /// <param name="reader">The <c>BinaryReader</c> to read from</param>
        /// <param name="filesystem">The <c>Filesystem</c> to modify based off what's read</param>
        private static void ReadFiles(BinaryReader reader, Filesystem filesystem)
        {
            var listEnd = false; // flag to stop loop at end of list
            while (!listEnd)
            {
                var fileStart = reader.ReadByte(); // file start flag
                var fileMeta = ReadGenericObjectMeta(reader); // generic meta properties
                var plaintext = Convert.ToBoolean(reader.ReadByte()); // Plaintext?

                var fileContentsLength = reader.ReadUInt32();
                var fileContents = reader.ReadBytes((int)fileContentsLength);
                var fileEnd = reader.ReadByte();

                var fileOwner = (Directory)filesystem.GetObjectById(fileMeta.OwnerId);
                if (fileOwner == null)
                {
                    // Error!
                }

                var newFile = new File(Encoding.UTF8.GetString(fileMeta.Title), fileMeta.Id, plaintext, fileContents);
                filesystem.AddObject(newFile, fileOwner);

                if (reader.ReadByte() == FileListEnd)
                {
                    listEnd = true;
                }
                else
                {
                    reader.BaseStream.Position -= 1;
                }
            }
        }

    }

    /// <summary>
    /// <c>GenericObjectMeta</c> is an internal class representing the properties common between File and Directory objects
    /// </summary>
    internal class GenericObjectMeta
    {
        public uint Id { get; set; }
        public uint OwnerId { get; set; }
        public ushort TitleLength { get; set; }
        public byte[] Title { get; set; }
    }
}
