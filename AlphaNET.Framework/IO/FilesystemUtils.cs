using System;
using System.IO;
using System.Runtime.Serialization;

namespace AlphaNET.Framework.IO
{
    public static class FilesystemUtils
    {
        public static bool IsFsFileInitialized(FileStream stream)
        {
            // by default, the status is initialized
            var initializedStatus = true;
            
            try
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
            catch (Exception e)
            {
                // getting an exception here probably means the file doesn't exist, or can't be accessed,
                // but it can't be initialized if it doesn't exist!
                // can also be a multitude of other things, but the filesystem's FileStream is opened almost identically to this,
                // so something is gonna end up wrong anyway lol
                Console.WriteLine(e);
                initializedStatus = false;
            }

            return initializedStatus;
        }

        /// <summary>
        /// Static wrapper method around the IFilesystemObject<T>.Deserialize() method.
        /// </summary>
        /// <typeparam name="T">The deserialized type to return</typeparam>
        /// <param name="buffer">The byte buffer to base the deserialization off</param>
        /// <param name="handler">A handler which uses the BinaryReader passed into it in order to deserialize the object</param>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(byte[] buffer, Func<BinaryReader, T> handler)
        {
            var stream = new MemoryStream(buffer);
            T returnObject;

            using (var reader = new BinaryReader(stream))
            {
                returnObject = handler(reader);
            }

            stream.Close();

            return returnObject;
        }

        /// <summary>
        /// Static wrapper method around the IFilesystemObject<T>.Serialize() method.
        /// </summary>
        /// <param name="handler">A handler which users the BinaryWriter passed to into it in order to serialize the object</param>
        /// <returns>The serialized object</returns>
        public static byte[] Serialize(Action<BinaryWriter> handler)
        {
            var stream = new MemoryStream();
            byte[] bytes;
            using (var writer = new BinaryWriter(stream))
            {
                // run provided handler to do all the writing
                handler(writer);
                bytes = stream.ToArray();
            }

            stream.Close();

            return bytes;
        }
    }
}