using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class RequestSocketStatusResponse : Packet
    {
        public SocketStatus SocketStatus { get; private set; }
        public Address RequestingAddress { get; private set; }

        public RequestSocketStatusResponse(SocketStatus socketStatus, Address requestingAddress)
        {
            SocketStatus = socketStatus;
            RequestingAddress = requestingAddress;
            Type = PacketType.REQUEST_SOCKET_STATUS_RESPONSE;
        }
    }
}
