using System;
using System.IO;

namespace AlphaNET.Framework.Net.Packets
{
    public class SocketConnectionStatus : Packet
    {
        public bool Connected { get; private set; }

        public SocketConnectionStatus(bool connected)
        {
            Connected = connected;
        }

        public override byte[] ToBytes()
        {
            byte[] bytes;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(PacketTypeCodes.SOCKET_CONNECTION_STATUS); // SocketStatus Packet Code
            writer.Write(Connected); // Connected
            bytes = stream.ToArray();
            writer.Close();

            return bytes;
        }

        public static SocketConnectionStatus FromBytes(byte[] bytes)
        {
            SocketConnectionStatus conStatus;
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            byte packetType = reader.ReadByte(); // Packet Type
            if (packetType != PacketTypeCodes.SOCKET_CONNECTION_STATUS)
            {
                return null;
            }
            bool connected = Convert.ToBoolean(reader.ReadByte());

            conStatus = new SocketConnectionStatus(connected);
            reader.Close();
            return conStatus;
        }
    }
}
