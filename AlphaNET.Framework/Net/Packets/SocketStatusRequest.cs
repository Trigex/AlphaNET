using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatusRequest : Packet
    {
        public Address SourceAddress { get; }
        public Address DestinationAddress { get; }

        public SocketStatusRequest(Address sourceAddress, Address destinationAddress)
        {
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            Type = PacketType.SOCKET_STATUS_REQUEST;
        }
    }
}
