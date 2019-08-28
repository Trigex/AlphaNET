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
    }
}