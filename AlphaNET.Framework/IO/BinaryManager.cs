using AlphaNET.Framework.Exceptions;
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
        private const byte FS_VERSION_TARGET = 1;
        // Section flag magic numbers
        private const byte FS_HEADER_START = 6;
        private const byte FS_HEADER_END = 9;
        private const byte DIR_LIST_START = 1;
        private const byte DIR_LIST_END = 2;
        private const byte FILE_LIST_START = 3;
        private const byte FILE_LIST_END = 4;
        private const byte DIR_START = 5;
        private const byte DIR_END = 6;
        private const byte FILE_START = 7;
        private const byte FILE_END = 8;

        /// <summary>
        /// Creates a binary encoded <c>Filesystem</c> from a <c>Filesystem</c> instance
        /// </summary>
        /// <param name="filesystem">The <c>Filesystem</c> to create the binary from</param>
        /// <returns>A byte array representing the binary encoded <c>Filesystem</c></returns>
        public static byte[] CreateBinaryFromFilesystem(Filesystem filesystem)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
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

            writer.Write(FS_HEADER_START); // FS Header start flag
            writer.Write(FS_VERSION_TARGET); // FS Version
            writer.Write(FS_HEADER_END); // FS Header end flag

            writer.Write(DIR_LIST_START); // Dir list start flag
            foreach (var dir in directories)
            {
                writer.Write(DIR_START); // dir start flag
                WriteGenericObjectMeta(writer, dir);
                writer.Write(DIR_END); // dir end flag
            }
            writer.Write(DIR_LIST_END); // Dir list end flag

            writer.Write(FILE_LIST_START);
            foreach (var file in files)
            {
                writer.Write(FILE_START); // file start flag
                WriteGenericObjectMeta(writer, file);
                writer.Write(file.IsPlaintext); // Plaintext?
                writer.Write((uint)file.Contents.Length); // Contents length
                writer.Write(file.Contents); // Contents
                writer.Write(FILE_END); // file end flag
            }
            writer.Write(FILE_LIST_END);
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
            MemoryStream stream = new MemoryStream(bin);
            BinaryReader reader = new BinaryReader(stream);
            Filesystem fs = new Filesystem();

            var headerStart = reader.ReadByte();
            if (headerStart != FS_HEADER_START) // FS File Header Start
            {
                throw new InvalidFilesystemHeaderException("Start", headerStart);
            }

            byte fsVersion = reader.ReadByte(); // FS Version

            var headerEnd = reader.ReadByte();
            if (headerEnd != FS_HEADER_END) // FS File Header End
            {
                throw new InvalidFilesystemHeaderException("End", headerEnd);
            }

            var dirListStart = reader.ReadByte();
            if (dirListStart != DIR_LIST_START)// Directory List Start
            {
                throw new InvalidFilesystemHeaderException("Directory List Start", dirListStart);
            }

            // TODO: Directory Exceptions

            ReadDirectories(reader, fs);

            // The ReadDirectories functions consumes DIR_LIST_END, in order to know when to stop, so it's not present here
            var fileListStart = reader.ReadByte();
            if (fileListStart != FILE_LIST_START)// File List Start
            {
                throw new InvalidFilesystemHeaderException("File List Start", fileListStart);
            }

            ReadFiles(reader, fs);

            // The ReadFiles functions consumes FILE_LIST_END, in order to know when to stop, so it's not present here
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
            writer.Write(fsObj.ID); // ID
            writer.Write(fsObj.Owner.ID); // OwnerID
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
            var genericObjectMeta = new GenericObjectMeta();

            genericObjectMeta.ID = reader.ReadUInt32();
            genericObjectMeta.OwnerID = reader.ReadUInt32();
            genericObjectMeta.TitleLength = reader.ReadUInt16();
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
                var dirStart = reader.ReadByte();
                var dirMeta = ReadGenericObjectMeta(reader);
                var dirEnd = reader.ReadByte();

                var newDir = new Directory(Encoding.UTF8.GetString(dirMeta.Title), dirMeta.ID);
                if (newDir.Title == "root")
                {
                    newDir.Owner = newDir;
                    filesystem.AddFilesystemObject(newDir);
                }
                else
                {
                    var dirOwner = (Directory)filesystem.GetFilesystemObjectByID(dirMeta.OwnerID);

                    if (dirOwner == null | dirOwner.GetType() != typeof(Directory))
                    {
                        // Error!
                    }

                    filesystem.AddFilesystemObject(newDir, dirOwner);
                }

                if (reader.ReadByte() == DIR_LIST_END)
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
            var listEnd = false;
            while (!listEnd)
            {
                var fileStart = reader.ReadByte();
                var fileMeta = ReadGenericObjectMeta(reader);
                var filePlaintext = reader.ReadByte();
                var plaintext = false;

                if (filePlaintext == 1)
                {
                    plaintext = true;
                }

                var fileContentsLength = reader.ReadUInt32();
                var fileContents = reader.ReadBytes((int)fileContentsLength);
                var fileEnd = reader.ReadByte();

                var fileOwner = (Directory)filesystem.GetFilesystemObjectByID(fileMeta.OwnerID);
                if (fileOwner == null)
                {
                    // Error!
                }

                var newFile = new File(Encoding.UTF8.GetString(fileMeta.Title), fileOwner, fileMeta.ID, plaintext, fileContents);
                filesystem.AddFilesystemObject(newFile, fileOwner);

                if (reader.ReadByte() == FILE_LIST_END)
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
    /// <c>GenericObjectMeta</c> is an internal class representing the data common between File and Directory objects
    /// </summary>
    internal class GenericObjectMeta
    {
        public uint ID { get; set; }
        public uint OwnerID { get; set; }
        public ushort TitleLength { get; set; }
        public byte[] Title { get; set; }
    }
}
