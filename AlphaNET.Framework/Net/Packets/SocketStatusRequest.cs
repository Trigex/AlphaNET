using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatusRequest : Packet
    {
        public Address requestedAddress { get; private set; }
        public Address requestingAddress { get; private set; }

        public SocketStatusRequest(Address requestedAddress, Address requestingAddress)
        {
            this.requestedAddress = requestedAddress;
            this.requestingAddress = requestingAddress;
            Type = PacketType.SOCKET_STATUS_REQUEST;
        }
    }
}
