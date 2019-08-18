using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatusResponse : Packet
    {
        public SocketStatus SocketStatus { get; private set; }
        public Address RequestingAddress { get; private set; }

        public SocketStatusResponse(SocketStatus socketStatus, Address requestingAddress)
        {
            SocketStatus = socketStatus;
            RequestingAddress = requestingAddress;
            Type = PacketType.SOCKET_STATUS_RESPONSE;
        }
    }
}
