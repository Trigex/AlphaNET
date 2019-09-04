using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// Static class which provides definitions for Filesystem-object related sizes
    /// </summary>
    public static class Size
    {
        public const int SUPER_BLOCK = 37;
        public const int BLOCK = 4096;
        public const int INODE = 132;
        public const int INODE_PER_BYTES = 16384;
        public const int INODES_PER_BLOCK = BLOCK / INODE;
        public const int FILE_TITLE = 128;
    }
}
