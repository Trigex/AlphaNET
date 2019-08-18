using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatusResponse : Packet
    {
        public SocketStatus SocketStatus { get; }
        public Address DestinationAddress { get; }

        public SocketStatusResponse(SocketStatus socketStatus, Address destinationAddress)
        {
            SocketStatus = socketStatus;
            DestinationAddress = destinationAddress;
            Type = PacketType.SOCKET_STATUS_RESPONSE;
        }
    }
}
