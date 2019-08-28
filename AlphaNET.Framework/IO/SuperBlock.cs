using System.IO;

namespace AlphaNET.Framework.IO
{
    public class SuperBlock
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

        public byte[] Serialize()
        {
            var stream = new MemoryStream();
            byte[] bytes;
            using (var writer = new BinaryWriter(stream))
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
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}