using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class SocketStatus : Packet
    {
        public bool Open { get; }
        public bool Listening { get; }

        public SocketStatus(bool open, bool listening)
        {
            Open = open;
            Listening = listening;
            Type = PacketType.SOCKET_STATUS;
        }
    }
}
