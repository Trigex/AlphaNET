namespace AlphaNET.Framework.IO
{
    public class Inode
    {
        public uint Number { get; set; }
        public uint BlockCount { get; set; }
        public uint TotalSize { get; set; }
        public ulong[] DirectDataBlockPointers { get; set; }
        public ulong PointerBlockPointer { get; set; }
        public ulong DoublePointerBlockPointer { get; set; }
    }
}