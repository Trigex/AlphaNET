using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketConnectionRequest : Packet
    {
        public Address remoteAddress { get; private set; }
        public Address requestingAddress { get; private set; }

        public SocketConnectionRequest(Address remoteAddress, Address requestingAddress)
        {
            this.remoteAddress = remoteAddress;
            this.requestingAddress = requestingAddress;
            Type = PacketType.SOCKET_CONNECTION_REQUEST;
        }
    }
}
