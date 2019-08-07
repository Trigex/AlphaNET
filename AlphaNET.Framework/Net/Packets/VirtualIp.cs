using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AlphaNET.Framework.Net.Packets
{
    public class VirtualIP : Packet
    {
        public string ip { get; private set; }

        public VirtualIP(string ip)
        {
            this.ip = ip;
        }

        public override byte[] ToBytes()
        {
            byte[] bytes;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(PacketType.VIRTUAL_IP); // VirtualIp Packet Code
            if (!(ip.Length > 32))
            {
                var encodedString = Encoding.UTF8.GetBytes(ip);
                writer.Write((byte)encodedString.Length); // IpAddressLength
                writer.Write(encodedString); // IpAddress
            }
            else
            {
                Debug.WriteLine("IP Address too large");
                return null;
            }
            bytes = stream.ToArray();
            writer.Close();

            return bytes;
        }

        public static VirtualIP FromBytes(byte[] bytes)
        {
            VirtualIP ip;

            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            if(reader.ReadByte() != PacketType.VIRTUAL_IP )
            {
                return null;
            }

            byte ipAddrLength = reader.ReadByte(); // IPAddressLength
            byte[] ipAddr = reader.ReadBytes(ipAddrLength); // IpAddress
            ip = new VirtualIP(Encoding.UTF8.GetString(ipAddr));
            reader.Close();
            return ip;
        }
    }
}
