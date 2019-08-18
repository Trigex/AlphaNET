using System;
using System.Net;

namespace AlphaNET.Server
{
    public static class Utils
    {
        /// <summary>
        /// Generates a random IpAddress
        /// </summary>
        /// <returns>String representation of the generated IpAddress</returns>
        public static string GenerateIpAddress()
        {
            var data = new byte[4];
            new Random().NextBytes(data);
            return new IPAddress(data).ToString();
        }

        /// <summary>
        /// Strip an <c>ipPort</c> string of it's port
        /// </summary>
        /// <param name="ipPort"></param>
        /// <returns></returns>
        public static string StripIpPort(string ipPort)
        {
            var colonIndex = ipPort.IndexOf(':');
            return ipPort.Remove(colonIndex);
        }
    }
}
