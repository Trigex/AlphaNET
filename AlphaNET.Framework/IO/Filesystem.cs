using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static System.String;

namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// The Filesystem class interacts with a .fs Filesystem file, retrieving, and writing data representing the Filesystem to and from the file.
    /// </summary>
    public class Filesystem
    {
        #region Dynamic Properties
        /// <summary>
        /// The path to the .fs file this Filesystem instance is working with
        /// </summary>
        private readonly string _fsPath;
        /// <summary>
        /// The total size (in bytes) of the .fs file to which this Filesystem instance is assigned
        /// </summary>
        private ulong _fsSize;
        /// <summary>
        /// The total count of blocks present in the .fs file
        /// </summary>
        private uint _blockCount;
        /// <summary>
        /// The total count of INodes present in the .fs file
        /// </summary>
        private uint _freeBlockCount;
        /// <summary>
        /// How many allocated Inodes in the Filesystem are free?
        /// </summary>
        private uint _freeInodeCount;
        /// <summary>
        /// How many Inodes are present in the Filesystem?
        /// </summary>
        private uint _inodeCount;
        /// <summary>
        /// Array containing pointers to the start of every block in the .fs file
        /// </summary>
        private ulong[] _blockPointers;
        /// <summary>
        /// The position of the Inode table section
        /// </summary>
        private ulong _inodeTablePointer;
        /// <summary>
        /// An instance of SuperBlock, reflecting properties of this Filesystem instance. Updated quite often, written to the .fs File's SuperBlock section
        /// </summary>
        private SuperBlock _superBlock;
        /// <summary>
        /// The FileStream this instance will constantly have open, for interaction with the .fs file
        /// </summary>
        private readonly FileStream _fsStream;
        /// <summary>
        /// The BinaryReader, opened from _fsStream, used for certain read operations
        /// </summary>
        private readonly BinaryReader _reader;
        /// <summary>
        /// The BinaryWriter, opened from _fsStream, used for certain write operations
        /// </summary>
        private readonly BinaryWriter _writer;
        #endregion
        
        #region Constant Properties
        /// <summary>
        /// The target .fs file version
        /// </summary>
        private const byte FsVersion = 1;
        /// <summary>
        /// The INode Number assigned to the Filesystem's root Directory
        /// </summary>
        private const int RootInodeNumber = 1;
        #endregion
        
        /// <summary>
        /// Instantiates a new Filesystem instance, and opens a FileStream
        /// </summary>
        /// <param name="fsPath">The path to the .fs file to open a FileStream from</param>
        public Filesystem(string fsPath)
        {
            _fsPath = fsPath;
            // opens the FileStream as ReadWrite, with NO file share permissions, in async mode
            // thus, things like AlphaNET Editor can't work with the .fs file unless
            // the AlphaNET client is closed. That sucks, I know, but given the nature of this, data can be incomplete while this instance is active,
            // and things could fall apart!
            _fsStream = new FileStream(_fsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true);
            _reader = new BinaryReader(_fsStream);
            _writer = new BinaryWriter(_fsStream);
        }
        
        /// <summary>
        /// Deconstructs the Filesystem instance, used to dispose our FileStream, BinaryReader, and BinaryWriter
        /// </summary>
        ~Filesystem()
        {
            Close();
        }

        public void Close()
        {
            _reader.Dispose();
            _writer.Dispose();
            _fsStream.Dispose();
        }

        #region Initialization Methods
        /// <summary>
        /// Initializes a new .fs file
        /// </summary>
        /// <param name="fsSize">The size (in bytes) of the Filesystem file</param>
        public async Task NewFilesystemAsync(ulong fsSize)
        {
            // Seeks to the last byte of the specified size
            SetStreamPosition(fsSize);
            // Writes a zero, which would actually fill the file with zeroes (supposedly, depends on the host filesystem I think)
            _fsStream.WriteByte(0);
            // set stream position back to 0
            ResetStreamPosition();
            
            Console.WriteLine("Wrote zeroes to file");
                
            // set fs properties
            _fsSize = fsSize;
            // blocks take up the entire .fs file
            _blockCount = (uint) (fsSize / Size.Block);
            Console.WriteLine($"Filesystem Block count: {_blockCount}");
            // TODO: Tweak this number, I sense something possibly going wrong
            _inodeCount = (uint) (fsSize / Size.InodePerBytes);
            Console.WriteLine($"Filesystem Inode count: {_inodeCount}");
            _freeBlockCount = _blockCount;
            _freeInodeCount = _inodeCount;
                
            // setup block pointers
            _blockPointers = new ulong[_blockCount];
            for (var i = 0; i < _blockPointers.Length; i++)
            {
                // the start of each block is the block index * BlockSize
                _blockPointers[i] = (ulong)(i * Size.Block);
            }
            
            Console.WriteLine($"{_blockPointers.Length} block pointers");

            // setup SuperBlock section
            _superBlock = CreateSuperBlock();
            var superBlockBuffer = _superBlock.Serialize();
                
            // SuperBlock gets written at the very start of the file
            await WriteToBlockAsync(superBlockBuffer, _blockPointers[0]);
            
            Console.WriteLine($"Wrote SuperBlock to block {_blockPointers[0]}");
            
            //Console.WriteLine(await ReadFromBlockAsync(_blockPointers[0]));
            
            // set inode table pointer property
            _inodeTablePointer = _blockPointers[1]; // INode table starts at the second block
            // amount of blocks needed to hold all inodes
            float blkCnt = _blockCount / (_inodeCount / Size.Inode);
            Console.WriteLine($"Blocks used for Inode Table: {blkCnt}");

            // list of all Inodes in the filesystem, serialized
            var completeInodeList = new List<byte[]>();
            var blankInode = Inode.GenerateEmptyInode();
            
            // Loop for all inodes
            for (int z = 1; z < _inodeCount; z++)
            {
                blankInode.Number = (uint)z;
                // add serialized Inode to list
                completeInodeList.Add(blankInode.Serialize());
            }
            
            Console.WriteLine("Serialized Inodes into Inode list");
            Console.WriteLine($"Size of an Inode: {completeInodeList[1].Length}");

            // fill blocks 1 to inodeTableBlockCount with inode buffers, containing 31 Inodes
            for (int i = 1; i < blkCnt; i++)
            {
                // get current table block inode range from completeInodeList
                var blockTable = completeInodeList.GetRange((i - 1) * Size.InodesPerBlock, Size.InodesPerBlock - 1);
                var buffer = new List<byte>();
                // copy block table into buffer
                foreach (var inode in blockTable)
                {
                    // copy inode's bytes into buffer
                    foreach (var _byte in inode)
                    {
                        buffer.Add(_byte);
                    }
                }

                // finally, write the table block!
                await WriteToBlockAsync(buffer.ToArray(), _blockPointers[i]);
            }
            
            // allocate root inode
            var rootNode = await GetFreeInode();
        }

        /// <summary>
        /// Loads state for the Filesystem from the Filesystem's .fs file
        /// </summary>
        /// <returns></returns>
        public async Task LoadFilesystemAsync()
        {
            if (!FilesystemUtils.IsFsFileInitialized(_fsStream)) { Console.WriteLine("FS File not init'd"); return; }

            // get superblock
            var superBuffer = await ReadFromBlockAsync(0);
            var fullBlock = new List<byte>(superBuffer);
            _superBlock = new SuperBlock().Deserialize(fullBlock.GetRange(0, Size.SuperBlock).ToArray());
        }
        #endregion

        #region Inode Management Methods
        /// <summary>
        /// Returns free Inode in the Inode table
        /// </summary>
        /// <returns>Empty INode</returns>
        private async Task<Inode> GetFreeInode()
        {
            Console.WriteLine($"Free Inode number: {((_inodeCount - _freeInodeCount) + 1)}");
            var inode = await GetInodeAsync((_inodeCount - _freeInodeCount) + 1);
            _freeInodeCount--;
            return inode;
        }
        
        /// <summary>
        /// Returns an Inode of the given Inode Number
        /// </summary>
        /// <param name="inodeNumber">The Inode Number of the Inode to return</param>
        /// <returns>Inode object of the matching Inode Number</returns>
        private async Task<Inode> GetInodeAsync(uint inodeNumber)
        {
            Console.WriteLine($"Attempting to get Inode Number {inodeNumber}");
            // get the block the inode is stored in
            var blockNumber = GetInodeTableBlock(inodeNumber);
            Console.WriteLine($"Inode block number: {blockNumber}");
            // get the index of the inode in it's block
            var tableIndex = GetInodeIndexInTableBlock(inodeNumber, blockNumber);
            Console.WriteLine($"Inode table index: {tableIndex}");
            // the inode table starts after the superblock, so our inode table block would be blockNumber
            var blockPointer = _blockPointers[blockNumber];
            Console.WriteLine($"Inode Block Pointer: {blockPointer}");
            // gets us to the first byte of the index Inode
            var inodePos = (inodeNumber - 1) * 132;
            Console.WriteLine($"Block Inode offsets: {inodePos}");
            // read inode bytes
            var buffer = await ReadBufferFromStreamAsync(blockPointer + inodePos, Size.Inode);
            Console.WriteLine(new Inode().Deserialize(buffer).Number);
            return new Inode().Deserialize(buffer);
        }

        /// <summary>
        /// Finds the index of an Inode in a Table Block 
        /// </summary>
        /// <param name="inodeNumber">The Inode Number of the Inode to find in the block</param>
        /// <param name="tableBlockNumber">The Inode Table Block to search for the Inode in</param>
        /// <returns>Index of the Inode object in the Table Block</returns>
        private uint GetInodeIndexInTableBlock(uint inodeNumber, uint tableBlockNumber)
        {
            return inodeNumber - (Size.InodesPerBlock * (tableBlockNumber - 1));
        }
        
        /// <summary>
        /// Returns the Inode Table Block containing an Inode of the given Inode Number
        /// </summary>
        /// <param name="inodeNumber">Inode to find the table block of</param>
        /// <returns>Table block number</returns>
        private uint GetInodeTableBlock(uint inodeNumber)
        {
            return (inodeNumber / Size.InodesPerBlock) + 1;
        }
        #endregion
        
        #region Block IO & Management Methods
        private async Task<byte[]> ReadFromBlockAsync(ulong blockPointer)
        {
            return await ReadBufferFromStreamAsync(blockPointer, Size.Block);
        }
        
        /// <summary>
        /// Writes a given byte buffer to the start of a block, at position blockPointer
        /// </summary>
        /// <param name="buffer">The byte buffer to be written</param>
        /// <param name="blockPointer">The block pointer, pointing to the start of a block, to write from</param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception in the event the buffer was larger in length than the static size of a block</exception>
        private async Task WriteToBlockAsync(byte[] buffer, ulong blockPointer)
        {
            // this method only supports writing directly to a single block, thus exception if the byte length is too large
            if (buffer.Length > Size.Block)
                throw new Exception($"The specified byte array to be written was larger than the size of a block! (bytes was {buffer.Length}, and BlockSize is {Size.Block})");
            
            // writes the buffer to the block at position block pointer value
            await WriteBufferToStreamAsync(blockPointer, buffer);
            
            // If the buffer is smaller than BlockSize, ensure the rest of the block is filled with zeroes
            if (buffer.Length < Size.Block)
            {
                // BlockSize - bytes.length would be the rest of the block that was not written to
                var fillSize = Size.Block - buffer.Length;
                // array filled with zeroes (I think the default value zeroes will suffice, but C# might bitch, conflicting reports!
                var fillArr = new byte[fillSize];
                await WriteBufferToStreamAsync(blockPointer + (ulong) buffer.Length + 1, fillArr);
            }

            _freeBlockCount--;
        }
        #endregion
        
        #region FileStream Management Methods
        /// <summary>
        /// Resets the FileStream's position back to zero
        /// </summary>
        private void ResetStreamPosition()
        {
            _fsStream.Position = 0;
        }
        
        /// <summary>
        /// Sets the FileStream's position to the specified zero based position, relative to the start of the file
        /// </summary>
        /// <param name="position">The zero based position to seek to</param>
        private void SetStreamPosition(ulong position)
        {
            // Seeks position relative to beginning of the stream
            _fsStream.Seek((long)position, SeekOrigin.Begin);
        }
        
        /// <summary>
        /// Writes the entire contents of a buffer to the stream, starting at the given position
        /// </summary>
        /// <param name="position">The position to start writing at</param>
        /// <param name="buffer">The buffer to write</param>
        /// <returns></returns>
        private async Task WriteBufferToStreamAsync(ulong position, byte[] buffer)
        {
            var initialPosition = _fsStream.Position;
            // set position
            SetStreamPosition(position);
            // writes entire buffer to stream from set position
            await _fsStream.WriteAsync(buffer, 0, buffer.Length);
            // reset position to value before write (probably remove this later? We'll see how things end up working out in how the filestream is handled :) )
            SetStreamPosition((ulong)initialPosition);
        }
        
        /// <summary>
        /// Reads bytes from the stream at the given position, of the given length
        /// </summary>
        /// <param name="position">The zero-based position to start reading from</param>
        /// <param name="length">The total length of bytes to read</param>
        /// <returns></returns>
        private async Task<byte[]> ReadBufferFromStreamAsync(ulong position, int length)
        {
            if (position > (ulong)_fsStream.Length) 
                throw new Exception("Attempting to read position outside of buffer length!");
            
            var initialPosition = _fsStream.Position;
            // create buffer to hold read bytes
            var buffer = new byte[length];
            // set position
            SetStreamPosition(position);
            // read into buffer
            await _fsStream.ReadAsync(buffer, 0, length);
            // reset position
            SetStreamPosition((ulong)initialPosition);
            return buffer;
        }
        #endregion

        /// <summary>
        /// Creates a SuperBlock object, based on the current Filesystem instance's properties
        /// </summary>
        /// <returns>The generated SuperBlock</returns>
        private SuperBlock CreateSuperBlock()
        {
            return new SuperBlock(FsVersion, _fsSize, Size.Block, Size.Inode, _blockCount, _inodeCount, _freeInodeCount, _freeBlockCount, Size.Block * 2);
        }
    }
}
