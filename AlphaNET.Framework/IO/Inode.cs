using System.IO;

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

        public byte[] Serialize()
        {
            var stream = new MemoryStream();
            byte[] bytes;
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(Number);
                writer.Write(BlockCount);
                writer.Write(TotalSize);
                // write all direct data block pointers
                for (int i = 0; i < DirectDataBlockPointers.Length; i++)
                {
                    writer.Write(DirectDataBlockPointers[i]);
                }
                writer.Write(PointerBlockPointer);
                writer.Write(DoublePointerBlockPointer);
                bytes = stream.ToArray();
            }

            return bytes;
        }

        public static Inode GenerateEmptyInode()
        {
            return new Inode
            {
                Number = 0,
                BlockCount = 0,
                DirectDataBlockPointers = new ulong[13],
                PointerBlockPointer = 0,
                DoublePointerBlockPointer = 0
            };
        }
    }
}