using System;

namespace AlphaNET.Framework.Exceptions
{
    class InvalidFilesystemHeaderException : Exception
    {
        public InvalidFilesystemHeaderException(string flag, byte found) : base(
            $"The Filesystem Header {flag} flag was not found, but instead found {found}")
        {

        }
    }
}
