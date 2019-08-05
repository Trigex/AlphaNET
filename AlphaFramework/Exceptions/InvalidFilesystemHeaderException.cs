using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaFramework.Exceptions
{
    class InvalidFilesystemHeaderException : Exception
    {
        public InvalidFilesystemHeaderException(string flag, byte found) : base(String.Format("The Filesystem Header {0} flag was not found, but instead found {1}", flag, found))
        {

        }
    }
}
