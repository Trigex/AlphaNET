using System.IO;

namespace AlphaNET.Framework.IO
{
    public class SuperBlock : IFilesystemObject<SuperBlock>
    {
        public byte FsVersion { get; set; }
        public ulong FsFileSize { get; set; }
        public ushort BlockSize { get; set; }
        public ushort InodeSize { get; set; }
        public uint BlockCount { get; set; }
        public uint InodeCount { get; set; }
        public uint FreeINodeCount { get; set; }
        public uint FreeBlockCount { get; set; }
        public ulong InodeTablePointer { get; set; }

        public SuperBlock(byte fsVersion, ulong fsFileSize, ushort blockSize, ushort inodeSize, uint blockCount, uint inodeCount, uint freeINodeCount, uint freeBlockCount, ulong inodeTablePointer)
        {
            FsVersion = fsVersion;
            FsFileSize = fsFileSize;
            BlockSize = blockSize;
            InodeSize = inodeSize;
            BlockCount = blockCount;
            InodeCount = inodeCount;
            FreeINodeCount = freeINodeCount;
            FreeBlockCount = freeBlockCount;
            InodeTablePointer = inodeTablePointer;
        }

        public SuperBlock() { }

        public SuperBlock Deserialize(byte[] buffer)
        {
            return FilesystemUtils.Deserialize(buffer, (reader) =>
            {
                var fsVersion = reader.ReadByte();
                var fsFileSize = reader.ReadUInt64();
                var blockSize = reader.ReadUInt16();
                var inodeSize = reader.ReadUInt16();
                var blockCount = reader.ReadUInt32();
                var inodeCount = reader.ReadUInt32();
                var freeBlockCount = reader.ReadUInt32();
                var freeInodeCount = reader.ReadUInt32();
                var inodeTablePointer = reader.ReadUInt64();

                return new SuperBlock(fsVersion, fsFileSize, blockSize, inodeSize, blockCount, inodeCount, freeInodeCount, freeBlockCount, inodeTablePointer);
            });
        }

        public byte[] Serialize()
        {
            return FilesystemUtils.Serialize((writer) =>
            {
                writer.Write(FsVersion);
                writer.Write(FsFileSize);
                writer.Write(BlockSize);
                writer.Write(InodeSize);
                writer.Write(BlockCount);
                writer.Write(InodeCount);
                writer.Write(FreeBlockCount);
                writer.Write(FreeINodeCount);
                writer.Write(InodeTablePointer);
            });
        }
    }
}