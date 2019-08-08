﻿using System;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public class VirtualIP : Packet
    {
        public string ip { get; private set; }

        public VirtualIP(string ip)
        {
            this.ip = ip;
            Type = PacketType.VIRTUAL_IP;
        }
    }
}
