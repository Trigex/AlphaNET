using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatusRequest : Packet
    {
        public Address SourceAddress { get; private set; }
        public Address DestinationAddress { get; private set; }

        public SocketStatusRequest(Address sourceAddress, Address destinationAddress)
        {
            this.SourceAddress = sourceAddress;
            this.DestinationAddress = destinationAddress;
            Type = PacketType.SOCKET_STATUS_REQUEST;
        }
    }
}
