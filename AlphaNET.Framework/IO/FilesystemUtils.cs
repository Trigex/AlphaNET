using System;
using System.IO;
using System.Runtime.Serialization;

namespace AlphaNET.Framework.IO
{
    public static class FilesystemUtils
    {
        public static bool IsFsFileInitialized(string fsFilePath)
        {
            // by default, the status is initialized
            var initializedStatus = true;
            
            try
            {
                using (var stream = new FileStream(fsFilePath, FileMode.Open, FileAccess.Read))
                {
                    // if the stream length isn't just 1 byte
                    if (stream.Length > 1)
                    {
                        stream.Position = 0;
                        // read FsByte, which is always the first byte of the file
                        byte fsVersion = (byte)stream.ReadByte();
                        
                        if (fsVersion == 0) // Zero ain't a valid FileSystem version. Safe bet it's not initialized
                            initializedStatus = false;
                    } else // basically empty file, makes you think
                        initializedStatus = false;
                }
            }
            catch (Exception e)
            {
                // getting an exception here probably means the file doesn't exist, or can't be accessed,
                // but it can't be initialized if it doesn't exist!
                // can also be a multitude of other things, but the filesystem's FileStream is opened almost identically to this,
                // so something is gonna end up wrong anyway lol
                initializedStatus = false;
            }

            return initializedStatus;
        }
    }
}