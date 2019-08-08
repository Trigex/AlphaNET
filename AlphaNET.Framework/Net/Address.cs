using System;

namespace AlphaNET.Framework.Net
{
    [Serializable]
    public class Address
    {
        public ushort Port { get; private set; }
        public string IpAddress { get; private set; }

        public Address(string ipAddress, ushort port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public override string ToString()
        {
            return IpAddress + ":" + Port.ToString();
        }
    }
}
