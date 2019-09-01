﻿using System.Collections.Generic;
using System.IO;
using Jint.Parser;

namespace AlphaNET.Framework.IO
{
    public class Inode : IFilesystemObject<Inode>
    {
        public uint Number { get; set; }
        public uint BlockCount { get; set; }
        public uint TotalSize { get; set; }
        public const int DirectDataBlockPointerCount = 13;
        public ulong[] DirectDataBlockPointers { get; set; }
        public ulong PointerBlockPointer { get; set; }
        public ulong DoublePointerBlockPointer { get; set; }

        public Inode(uint number)
        {
            Number = number;
        }
        
        public Inode(uint number, uint blockCount, uint totalSize, ulong[] directDataBlockPointers, ulong pointerBlockPointer, ulong doublePointerBlockPointer)
        {
            Number = number;
            BlockCount = blockCount;
            TotalSize = totalSize;
            DirectDataBlockPointers = directDataBlockPointers;
            PointerBlockPointer = pointerBlockPointer;
            DoublePointerBlockPointer = doublePointerBlockPointer;
        }

        public Inode() { }

        public byte[] Serialize()
        {
            return FilesystemUtils.Serialize((writer) =>
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
            });
        }

        public Inode Deserialize(byte[] buffer)
        {
            return FilesystemUtils.Deserialize(buffer, (reader) =>
            {
                var number = reader.ReadUInt32();
                var blockCount = reader.ReadUInt32();
                var totalSize = reader.ReadUInt32();
                var dataBlockPointers = new List<ulong>();
                for (int i = 0; i < DirectDataBlockPointerCount; i++)
                {
                    dataBlockPointers.Push(reader.ReadUInt64());
                }

                var pointerBlockPointer = reader.ReadUInt64();
                var doublePointerBlockPointer = reader.ReadUInt64();
                var output = new Inode(number, blockCount, totalSize, dataBlockPointers.ToArray(), pointerBlockPointer, doublePointerBlockPointer);

                return output;
            });
        }

        public static Inode GenerateEmptyInode()
        {
            return new Inode(0, 0, 0, new ulong[DirectDataBlockPointerCount], 0, 0);
        }
    }
}