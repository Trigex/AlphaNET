using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketConnectionRequest : Packet
    {
        public Address SourceAddress { get; private set; }
        public Address DestinationAddress { get; private set; }

        public SocketConnectionRequest(Address sourceAddress, Address destinationAddress)
        {
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            Type = PacketType.SOCKET_CONNECTION_REQUEST;
        }
    }
}
