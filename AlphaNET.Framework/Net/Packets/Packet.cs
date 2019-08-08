using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public abstract class Packet
    {
        public byte Type { get; protected set; }
    }
}
