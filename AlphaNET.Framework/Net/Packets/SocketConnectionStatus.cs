using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketConnectionStatus : Packet
    {
        public bool Connected { get; private set; }

        public SocketConnectionStatus(bool connected)
        {
            Connected = connected;
            Type = PacketType.SOCKET_CONNECTION_STATUS;
        }
    }
}
