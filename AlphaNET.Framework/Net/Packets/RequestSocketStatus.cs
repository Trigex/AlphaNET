using System.Collections.Generic;
using System.IO;

namespace AlphaNET.Framework.Net.Packets
{
    public class RequestSocketStatus : Packet
    {
        public Address remoteAddress { get; private set; }

        public RequestSocketStatus(Address remoteAddress)
        {
            this.remoteAddress = remoteAddress;
        }

        public override byte[] ToBytes()
        {
            byte[] bytes;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(PacketTypeCodes.REQUEST_SOCKET_STATUS); // RequestSocketStatus Packet Code
            writer.Write(remoteAddress.ToBytes()); // IpAddressLength(byte) IpAddress(UTF8 bytes) Port(uint16)
            bytes = stream.ToArray();
            writer.Close();

            return bytes;
        }

        public static RequestSocketStatus FromBytes(byte[] bytes)
        {
            RequestSocketStatus requestSocket;
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            byte packetType = reader.ReadByte(); // Packet Type
            if (packetType != PacketTypeCodes.REQUEST_SOCKET_STATUS)
            {
                return null;
            }
            // get bytes 
            var byteList = new List<byte>(bytes);
            byteList.RemoveAt(0); // Strip first byte (packet code) from array

            var addr = Address.FromBytes(byteList.ToArray());

            requestSocket = new RequestSocketStatus(addr);
            reader.Close();
            return requestSocket;
        }
    }
}
