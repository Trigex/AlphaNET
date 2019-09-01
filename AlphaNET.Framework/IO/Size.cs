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
        public const int SuperBlock = 37;
        public const int Block = 4096;
        public const int Inode = 132;
        public const int InodePerBytes = 16384;
        public const int InodesPerBlock = Block / Inode;
        public const int FileTitle = 128;
    }
}
