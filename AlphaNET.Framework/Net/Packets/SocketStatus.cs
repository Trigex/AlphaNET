using System;
using System.IO;

namespace AlphaNET.Framework.Net.Packets
{
    public class SocketStatus : Packet
    {
        public bool Open { get; private set; }
        public bool Listening { get; private set; }

        public SocketStatus(bool open, bool listening)
        {
            Open = open;
            Listening = listening;
        }
        public override byte[] ToBytes()
        {
            byte[] bytes;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(PacketType.SOCKET_STATUS); // SocketStatus Packet Code
            writer.Write(Open); // Open
            writer.Write(Listening); // Listening
            bytes = stream.ToArray();
            writer.Close();
            return bytes;
        }
        public static SocketStatus FromBytes(byte[] bytes)
        {
            SocketStatus socketStatus;
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            byte packetCode = reader.ReadByte();
            if (packetCode != PacketType.SOCKET_STATUS)
            {
                return null;
            }
            byte open = reader.ReadByte();
            byte listening = reader.ReadByte();
            socketStatus = new SocketStatus(Convert.ToBoolean(open), Convert.ToBoolean(listening));
            reader.Close();
            return socketStatus;
        }
    }
}
