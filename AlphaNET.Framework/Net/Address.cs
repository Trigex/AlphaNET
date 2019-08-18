using System;

namespace AlphaNET.Framework.Net
{
    [Serializable]
    public class Address
    {
        public ushort Port { get; }
        public string IpAddress { get; }

        public Address(string ipAddress, ushort port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public override string ToString()
        {
            return IpAddress + ":" + Port;
        }
    }
}
