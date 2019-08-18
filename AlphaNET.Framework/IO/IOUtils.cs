﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AlphaNET.Framework.IO
{
    /// <summary>
    /// Static class with util methods for IO operations
    /// </summary>
    public static class IOUtils
    {
        /// <summary>
        /// Generates a random <c>uint32</c>, used to uniquely identify <c>FilesystemObject</c>s
        /// </summary>
        /// <returns></returns>
        public static uint GenerateId()
        {
            var random = new Random();
            var thirtyBits = (uint)random.Next(1 << 30);
            var twoBits = (uint)random.Next(1 << 2);
            return (thirtyBits << 2) | twoBits;
        }

        public static string ReadManifestData<TSource>(string embeddedFileName) where TSource : class
        {
            var assembly = typeof(TSource).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().First(s => s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
