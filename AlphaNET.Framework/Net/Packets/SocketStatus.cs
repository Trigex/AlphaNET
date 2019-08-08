using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatus : Packet
    {
        public bool Open { get; private set; }
        public bool Listening { get; private set; }

        public SocketStatus(bool open, bool listening)
        {
            Open = open;
            Listening = listening;
            Type = PacketType.SOCKET_STATUS;
        }
    }
}
