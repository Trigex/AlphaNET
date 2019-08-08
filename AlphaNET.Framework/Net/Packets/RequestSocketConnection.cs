using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class RequestSocketConnection : Packet
    {
        public Address remoteAddress { get; private set; }
        public Address requestingAddress { get; private set; }

        public RequestSocketConnection(Address remoteAddress, Address requestingAddress)
        {
            this.remoteAddress = remoteAddress;
            this.requestingAddress = requestingAddress;
            Type = PacketType.REQUEST_SOCKET_CONNECTION;
        }
    }
}
