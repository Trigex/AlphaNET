using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlphaNET.Framework.Net.Packets
{
    public class RequestSocketConnection : Packet
    {
        public Address remoteAddress { get; private set; }
        public Address requestingAddress { get; private set; }

        public RequestSocketConnection(Address remoteAddress, Address requestingAddress)
        {
            this.remoteAddress = remoteAddress;
            this.requestingAddress = requestingAddress;
        }

        public override byte[] ToBytes()
        {
            byte[] bytes;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(PacketType.REQUEST_SOCKET_CONNECTION); // RequestSocketConnection Packet Code
            writer.Write(remoteAddress.ToBytes()); // Remote Address
            writer.Write(requestingAddress.ToBytes()); // Requesting Address
            bytes = stream.ToArray();
            writer.Close();
            return bytes;
        }

        public static RequestSocketConnection FromBytes(byte[] bytes)
        {
            RequestSocketConnection requestCon;
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            byte packetCode = reader.ReadByte();
            if (packetCode != PacketType.REQUEST_SOCKET_CONNECTION)
            {
                return null;
            }
            var addrList = new List<Address>();
            for (int i = 0; i >= 1; i++)
            {
                var AddrLength = reader.ReadByte();
                var AddrIp = reader.ReadBytes(AddrLength);
                var AddrPort = reader.ReadUInt16();
                addrList.Add(new Address(Encoding.UTF8.GetString(AddrIp), AddrPort));
            }

            requestCon = new RequestSocketConnection(addrList[0], addrList[1]);
            reader.Close();
            return requestCon;
        }
    }
}
