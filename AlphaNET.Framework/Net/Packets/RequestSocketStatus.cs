using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class RequestSocketStatus : Packet
    {
        public Address requestedAddress { get; private set; }
        public Address requestingAddress { get; private set; }

        public RequestSocketStatus(Address requestedAddress, Address requestingAddress)
        {
            this.requestedAddress = requestedAddress;
            this.requestingAddress = requestingAddress;
            Type = PacketType.REQUEST_SOCKET_STATUS;
        }
    }
}
