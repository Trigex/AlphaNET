using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        /// The size (in bytes) of a SuperBlock .fs file section
        /// </summary>
        private const int SuperBlockSectionSize = 37;
        /// <summary>
        /// The target .fs file version
        /// </summary>
        private const byte FsVersion = 1;
        /// <summary>
        /// The size (in bytes) of a single block in the .fs file
        /// </summary>
        private const int BlockSize = 4096;
        /// <summary>
        /// The size (in bytes) of a single INode object in the .fs file
        /// </summary>
        private const int InodeSize = 132;
        /// <summary>
        /// The INode Number assigned to the Filesystem's root Directory
        /// </summary>
        private const int RootInodeNumber = 1;
        /// <summary>
        /// The ratio of how many Inodes are created per bytes (Currently, per 16kb)
        /// </summary>
        private const int InodePerByteRatio = 16384;
        /// <summary>
        /// How many Inodes can fit into a single block?
        /// </summary>
        private const int InodesPerBlock = BlockSize / InodeSize;
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
            _fsStream = new FileStream(_fsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, true);
            _reader = new BinaryReader(_fsStream);
            _writer = new BinaryWriter(_fsStream);
        }
        
        /// <summary>
        /// Deconstructs the Filesystem instance, used to dispose our FileStream, BinaryReader, and BinaryWriter
        /// </summary>
        ~Filesystem()
        {
            _reader.Dispose();
            _writer.Dispose();
            _fsStream.Dispose();
        }
        
        /// <summary>
        /// Initializes a new .fs file
        /// </summary>
        /// <param name="fsSize">The size (in bytes) of the Filesystem file</param>
        public async Task InitializeFilesystemAsync(ulong fsSize)
        {
            // Seeks to the last byte of the specified size
            SetStreamPosition(fsSize);
            // Writes a zero, which would actually fill the file with zeroes (supposedly, depends on the host filesystem I think)
            _fsStream.WriteByte(0);
            // set stream position back to 0
            ResetStreamPosition();
                
            // set fs properties
            _fsSize = fsSize;
            // blocks take up the entire .fs file
            _blockCount = (uint) (fsSize / BlockSize);
            // TODO: Tweak this number, I sense something possibly going wrong
            _inodeCount = (uint) (fsSize / InodePerByteRatio);
            _freeBlockCount = _blockCount;
            _freeInodeCount = _inodeCount;
                
            // setup block pointers
            _blockPointers = new ulong[_blockCount];
            for (var i = 0; i < _blockPointers.Length; i++)
            {
                // the start of each block is the block index * BlockSize
                _blockPointers[i] = (ulong)(i * BlockSize);
            }

            // setup SuperBlock section
            var superBlock = GenerateSuperBlock();
            var superBlockBuffer = superBlock.Serialize();
                
            // SuperBlock gets written at the very start of the file
            await WriteToBlockAsync(superBlockBuffer, 0);

            _inodeTablePointer = _blockPointers[1]; // INode table starts at the second block
            // Create Inode table starting at the file's second block, fill with empty inodes
            var emptyInode = Inode.GenerateEmptyInode().Serialize();
            for (int i = 1; i <= _inodeCount; i++)
            {
                WriteBufferToStreamAsync()
            }
        }
        
        #region Block IO & Management Methods
        private async Task<byte[]> ReadFromBlockAsync(ulong blockPointer)
        {
            var buffer = new byte[BlockSize];
            // Reads BlockSize count bytes from the the start of the block pointer to the end
            await _fsStream.ReadAsync(buffer, 0, BlockSize);
            return buffer;
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
            if (buffer.Length > BlockSize)
                throw new Exception($"The specified byte array to be written was larger than the size of a block! (bytes was {buffer.Length}, and BlockSize is {BlockSize})");
            
            // writes the buffer to the block at position block pointer value
            await WriteBufferToStreamAsync(blockPointer, buffer);
            
            // If the buffer is smaller than BlockSize, ensure the rest of the block is filled with zeroes
            if (buffer.Length < BlockSize)
            {
                // BlockSize - bytes.length would be the rest of the block that was not written to
                var fillSize = BlockSize - buffer.Length;
                // array filled with zeroes (I think the default value zeroes will suffice, but C# might bitch, conflicting reports!
                var fillArr = new byte[fillSize];
                await WriteBufferToStreamAsync(blockPointer + (ulong) buffer.Length + 1, fillArr);
            }
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
        /// <param name="position">The position to start reading from</param>
        /// <param name="length">The total length of bytes to read</param>
        /// <returns></returns>
        private async Task<byte[]> ReadBufferFromStreamAsync(ulong position, int length)
        {
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
        /// Generates a SuperBlock object, based on the current Filesystem instance's properties
        /// </summary>
        /// <returns>The generated SuperBlock</returns>
        private SuperBlock GenerateSuperBlock()
        {
            return new SuperBlock
            {
                BlockCount = (uint)_blockCount,
                BlockSize = BlockSize,
                FreeBlockCount = (uint)_freeBlockCount,
                FreeINodeCount = (uint)_freeInodeCount,
                FsFileSize = (ulong)_fsSize,
                FsVersion = FsVersion,
                InodeCount = (uint)_inodeCount,
                InodeSize = InodeSize,
                InodeTablePointer = SuperBlockSectionSize + 1
            };
        }
    }
}
