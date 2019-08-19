﻿using AlphaNET.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    directories.Add((Directory) fsObj);
                }

                if (fsObj.GetType() == typeof(File))
                {
                    files.Add((File) fsObj);
                }
            }

            writer.Write(FsHeaderStart); // FS Header start flag
            writer.Write(FsVersionTarget); // FS Version
            writer.Write(FsHeaderEnd); // FS Header end flag

            writer.Write(DirListStart); // Dir list start flag
            foreach (var dir in directories)
            {
                WriteDirectory(dir, writer);
            }

            writer.Write(DirListEnd); // Dir list end flag

            writer.Write(FileListStart);
            foreach (var file in files)
            {
                WriteFile(file, writer);
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
        public static Filesystem CreateFilesystemFromBinary(byte[] bin, string fsPath)
        {
            if (bin == null) throw new ArgumentNullException(nameof(bin));
            if (fsPath == null) throw new ArgumentNullException(nameof(fsPath));
            var stream = new MemoryStream(bin);
            var reader = new BinaryReader(stream);
            var fs = new Filesystem(fsPath);

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
            if (dirListStart != DirListStart) // Directory List Start
            {
                throw new InvalidFilesystemHeaderException("Directory List Start", dirListStart);
            }

            // TODO: Directory Exceptions

            ReadDirectories(reader, fs);

            // The ReadDirectories method consumes DIR_LIST_END, in order to know when to stop, so it's not present here
            var fileListStart = reader.ReadByte();
            if (fileListStart != FileListStart) // File List Start
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
            filesystem = CreateFilesystemFromBinary(bin, filesystem._fsPath);
        }

        public static void InsertFilesystemObjectIntoBinary(FilesystemObject obj, string path)
        {
            var bin = ReadBinaryFromFile(path);
            bin = InsertFilesystemObjectIntoBinary(obj, bin);
            WriteBinaryToFile(path, bin);
        }

        public static byte[] InsertFilesystemObjectIntoBinary(FilesystemObject obj, byte[] bin)
        {
            var stream = new MemoryStream(bin);
            // create reader and writer from stream
            var reader = new BinaryReader(stream);
            var writer = new BinaryWriter(stream);
            // buffer to hold the encoded object to write
            byte[] encodedObj = null;

            if (obj.GetType() == typeof(Directory))
            {
                // Set position to right after DirListStart
                SeekFlag(reader, DirListStart);
                // Encode as Directory
                encodedObj = EncodeDirectory((Directory) obj);
            }
            else if (obj.GetType() == typeof(File))
            {
                // Set position to right after FileListStart
                SeekFlag(reader, FileListStart);
                // Encode as File
                encodedObj = EncodeFile((File) obj);
            }
            // create list from stream for ease of inserting
            var buffer = new List<byte>(stream.ToArray());
            // insert into list from current position, which would be right before a ListEnd
            buffer.InsertRange((int)stream.Position, encodedObj ?? throw new InvalidOperationException());
            // save the new binary
            var modified = buffer.ToArray();
            Console.WriteLine($"Added {obj.GetType()} at position {stream.Position}, of length {encodedObj.Length}");
            reader.Close();
            writer.Close();
            return modified;
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
        /// Loops through the DirectoriesList of a binary encoded <c>Filesystem</c>, modifying the <c>Filesystem</c> instance procedurely based off what it reads
        /// </summary>
        /// <param name="reader">The <c>BinaryReader</c> to read from</param>
        /// <param name="filesystem">The <c>Filesystem</c> to modify based off what's read</param>
        private static void ReadDirectories(BinaryReader reader, Filesystem filesystem)
        {
            var listEnd = false;
            while (!listEnd)
            {
                reader.ReadByte(); // dir start
                var dirMeta = ReadGenericObjectMeta(reader);
                reader.ReadByte(); // dir end

                var newDir = new Directory(Encoding.UTF8.GetString(dirMeta.Title), dirMeta.Id);
                Console.WriteLine($"Title: {newDir.Title}, ID: {newDir.Id}, OwnerDir: {dirMeta.OwnerId}");
                if (newDir.Title == "root")
                {
                    newDir.Owner = newDir;
                    filesystem.AddObject(newDir);
                }
                else
                {
                    var dirOwner = (Directory) filesystem.GetObjectById(dirMeta.OwnerId);
                    Console.WriteLine(dirOwner);
                    if (dirOwner == null || dirOwner.GetType() != typeof(Directory))
                    {
                        // Error!
                        throw new Exception($"Directory \"{newDir.Title}\"'s parent Directory could not be found!");
                    } else
                    {
                        filesystem.AddObject(newDir, dirOwner);
                    }
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

        private static void SkipDirectories(BinaryReader reader)
        {
            var listEnd = false;
            if (reader.ReadByte() != DirListStart)
            {
                reader.BaseStream.Position -= 1;
                return; // incorrect position, hmm
            }

            if (reader.ReadByte() != DirStart)
            {
                reader.BaseStream.Position -= 1;
                return; // The directory list is empty, can return
            }
                
            reader.BaseStream.Position -= 1; // go back to first dir start

            while (!listEnd)
            {
                var dirStart = reader.ReadByte();

                if (dirStart != DirStart) // dir start
                    Console.WriteLine($"What the fuck, {dirStart}");

                ReadGenericObjectMeta(reader);
                reader.ReadByte(); // dir end

                if (reader.ReadByte() == DirListEnd)
                {
                    listEnd = true;
                }
                else
                {
                    reader.BaseStream.Position -= 1; // go back a byte
                }
            }

            reader.ReadByte(); // consume dir list end
        }

        /// <summary>
        /// Takes the given BinaryReader to the end of the FileList
        /// </summary>
        /// <param name="reader">Reader to set the position of</param>
        private static void SkipFiles(BinaryReader reader)
        {
            var listEnd = false;
            if (reader.ReadByte() != FileListStart) return; // incorrect position, hmm
            if (reader.ReadByte() != FileStart) return; // The directory list is empty, can return
            reader.BaseStream.Position -= 1; // go back to first file start

            while (!listEnd)
            {
                var fileStart = reader.ReadByte();

                if (fileStart != FileStart) // file start
                    Console.WriteLine($"What the fuck, {fileStart}");

                ReadGenericObjectMeta(reader);
                reader.ReadByte(); // plaintext?
                var len = reader.ReadUInt16(); // contents length
                // change this later to just skip
                reader.ReadBytes(len); // contents
                reader.ReadByte(); // file end

                if (reader.ReadByte() == FileListEnd)
                {
                    listEnd = true;
                }
                else
                {
                    reader.BaseStream.Position -= 1; // go back a byte
                }
            }

            reader.ReadByte(); // consume file list end
        }

        /// <summary>
        /// Loops through the FilesList of a binary encoded <c>Filesystem</c>, modifying the <c>Filesystem</c> instance procedurally based off what it reads
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
                var fileContents = reader.ReadBytes((int) fileContentsLength);
                var fileEnd = reader.ReadByte();

                var fileOwner = (Directory) filesystem.GetObjectById(fileMeta.OwnerId);
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

        private static void WriteFile(File file, BinaryWriter writer)
        {
            writer.Write(EncodeFile(file));
        }

        private static void WriteDirectory(Directory directory, BinaryWriter writer)
        {
            writer.Write(EncodeDirectory(directory));
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
                Id = reader.ReadUInt32(),
                OwnerId = reader.ReadUInt32(),
                TitleLength = reader.ReadUInt16()
            };

            genericObjectMeta.Title = reader.ReadBytes(genericObjectMeta.TitleLength);

            return genericObjectMeta;
        }

        private static byte[] EncodeFile(File file)
        {
            byte[] encoded;
            var memoryStream = new MemoryStream();

            using (var writer = new BinaryWriter(memoryStream))
            {
                // encode file
                writer.Write(FileStart); // file start flag
                WriteGenericObjectMeta(writer, file);
                writer.Write(file.IsPlaintext); // Plaintext?
                writer.Write((uint)file.Contents.Length); // Contents length
                writer.Write(file.Contents); // Contents
                writer.Write(FileEnd); // file end flag

                encoded = memoryStream.ToArray();
            }

            return encoded;
        }

        private static byte[] EncodeDirectory(Directory directory)
        {
            byte[] encoded;
            var memoryStream = new MemoryStream();

            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write(DirStart); // dir start flag
                WriteGenericObjectMeta(writer, directory);
                writer.Write(DirEnd); // dir end flag

                encoded = memoryStream.ToArray();
            }

            return encoded;
        }

        private static void SeekFlag(BinaryReader reader, byte flag)
        {
            if (flag == DirListStart)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin); // start at begining
                reader.BaseStream.Seek(3, SeekOrigin.Current); // Skip FS File header
                //reader.BaseStream.Seek(1, SeekOrigin.Current); // skip dir list start
                SkipDirectories(reader);
                //reader.BaseStream.Position -= 1;  // go back 2, taking us to DirEnd of the last Dir in DirList
                // reached dir list!
            }
            else if (flag == FileListStart)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.BaseStream.Seek(3, SeekOrigin.Current); // Skip FS File headers
                SkipDirectories(reader); // skip the directories list
                SkipFiles(reader); // skip file list
                //reader.BaseStream.Position -= 1; // go back 2, so we're at the FileEnd of the last File in FileList 
                // reached file list!
            }
        }

        public static void PrintFilesystem(byte[] bin)
        {
            using (var reader = new BinaryReader(new MemoryStream(bin)))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                Console.WriteLine($"FS File Header Start: {reader.ReadByte()}");
                Console.WriteLine($"FS Version: {reader.ReadByte()}");
                Console.WriteLine($"FS File Header End: {reader.ReadByte()}");
                Console.WriteLine($"Dir List Start: {reader.ReadByte()}");
                while (reader.ReadByte() != DirListEnd)
                {
                    reader.BaseStream.Position -= 1;
                    Console.WriteLine($"Directory Start: {reader.ReadByte()}");
                    Console.WriteLine($"Directory ID: {reader.ReadUInt32()}");
                    Console.WriteLine($"Directory Owner ID: {reader.ReadUInt32()}");
                    var dirTitleLength = reader.ReadUInt16();
                    Console.WriteLine($"Directory Title Length: {dirTitleLength}");
                    Console.WriteLine($"Directory Title: {Encoding.UTF8.GetString(reader.ReadBytes(dirTitleLength))}");
                    Console.WriteLine($"Directory End: {reader.ReadByte()}");
                }

                reader.BaseStream.Position -= 1; // set back to dir list end
                Console.WriteLine($"Dir List End: {reader.ReadByte()}");
                Console.WriteLine($"File List Start: {reader.ReadByte()}");
                while (reader.ReadByte() != FileListEnd)
                {
                    reader.BaseStream.Position -= 1;
                    Console.WriteLine($"File Start: {reader.ReadByte()}");
                    Console.WriteLine($"File ID: {reader.ReadUInt32()}");
                    Console.WriteLine($"File Owner ID: {reader.ReadUInt32()}");
                    var fileTitleLength = reader.ReadUInt16();
                    Console.WriteLine($"File Title Length: {fileTitleLength}");
                    Console.WriteLine($"File Title: {Encoding.UTF8.GetString(reader.ReadBytes(fileTitleLength))}");
                    Console.WriteLine($"File Plaintext?: {reader.ReadByte()}");
                    var fileContentsLength = reader.ReadUInt32();
                    Console.WriteLine($"File Contents Length: {fileContentsLength}");
                    var contents = reader.ReadBytes((int)fileContentsLength);
                    Console.WriteLine($"File Contents: {contents.Length}");
                    Console.WriteLine($"File End: {reader.ReadByte()}");
                }

                reader.BaseStream.Position -= 1;
                Console.WriteLine($"File List End: {reader.ReadByte()}");
            }
        }

        /// <summary>
        /// Get the total size of a Filesystem object as it would be represented
        /// in the binary filesystem format
        /// </summary>
        /// <param name="filesystem">The Filesystem instance to get the size of</param>
        /// <returns>The total size of the Filesystem</returns>
        private static int GetSize(Filesystem filesystem)
        {
            var size = 0;

            foreach (var obj in filesystem.FilesystemObjects)
            {
                if (obj.GetType() == typeof(File))
                    size += GetSize((File) obj);
                else if (obj.GetType() == typeof(Directory))
                    size += GetSize((Directory) obj);
            }

            size += sizeof(byte) * 7; // 7 1 byte fs flags in total
            return size;
        }

        private static int GetSize(Directory[] dirs)
        {
            var size = dirs.Sum(GetSize);

            return size + (sizeof(byte) * 2); // list start and end flags
        }

        private static int GetSize(File[] files)
        {
            var size = files.Sum(GetSize);

            return size + (sizeof(byte) * 2); // list start and end flags
        }

    /// <summary>
        /// Get the total size of a single File as it would be represented in
        /// the binary filesystem format
        /// </summary>
        /// <param name="file">The File instance to get the size of</param>
        /// <returns>The total size of the File</returns>
        private static int GetSize(File file)
        {
            return (sizeof(byte) * 3) + // File start, end, is plaintext
                   GetSize(CreateGenericObjectMeta(file)) + // generic object meta
                   sizeof(uint) + // content length
                   file.Contents.Length; // contents
        }

        /// <summary>
        /// Get the total size of a single Directory as it would be represented in
        /// the binary filesystem format
        /// </summary>
        /// <param name="directory">The Directory instance to get the size of</param>
        /// <returns>The total size of the Directory</returns>
        private static int GetSize(Directory directory)
        {
            return (sizeof(byte) * 2) + // dir start and end flag
                   GetSize(CreateGenericObjectMeta(directory));
        }

        /// <summary>
        /// Get the total size of a GenericObjectMeta instance, as it would
        /// be represented in the binary filesystem format
        /// </summary>
        /// <param name="meta">The GenericObjectMeta instance to get the size of</param>
        /// <returns>The total size of the GenericObjectMeta</returns>
        private static int GetSize(GenericObjectMeta meta)
        {
            return (sizeof(uint) * 2) + // id and owner id
                   sizeof(ushort) + // title length
                   meta.Title.Length; // size of the title
        }

        /// <summary>
        /// Create a GenericObjectMeta instance from a FilesystemObject instance
        /// </summary>
        /// <param name="obj">The FilesystemObject to create the GenericObjectMeta from</param>
        /// <returns>The new GenericObjectMeta instance</returns>
        private static GenericObjectMeta CreateGenericObjectMeta(FilesystemObject obj)
        {
            return new GenericObjectMeta
                {Id = obj.Id, OwnerId = obj.Owner.Id, Title = Encoding.UTF8.GetBytes(obj.Title), TitleLength = (ushort)Encoding.UTF8.GetByteCount(obj.Title)};
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
