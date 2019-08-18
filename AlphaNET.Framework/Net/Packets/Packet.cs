using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AlphaNET.Framework.Net.Packets
{
    [Serializable]
    public abstract class Packet
    {
        public byte Type { get; protected set; }
    }

    public static class PacketUtils
    {
        public static byte[] Serialize(Packet packet)
        {
            // Serialize packet, store in MemoryStream
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, packet);
            // create list from stream
            var dataList = new List<byte>(stream.ToArray());
            stream.Close(); // close
            dataList.Insert(0, packet.Type); // set first byte to packet type
            return dataList.ToArray(); // return bytes
        }

        public static Packet Deserialize(byte[] bytes)
        {
            // strip packet type byte before inserting into memory stream
            var byteList = new List<byte>(bytes);
            byteList.RemoveAt(0);
            var stream = new MemoryStream(byteList.ToArray());
            var formatter = new BinaryFormatter();
            Packet packet = null;

            switch(bytes[0])
            {
                case PacketType.SOCKET_CONNECTION_REQUEST:
                    packet = (SocketConnectionRequest)formatter.Deserialize(stream);
                    break;
                case PacketType.SOCKET_STATUS_REQUEST:
                    packet = (SocketStatusRequest)formatter.Deserialize(stream);
                    break;
                case PacketType.SOCKET_STATUS_RESPONSE:
                    packet = (SocketStatusResponse)formatter.Deserialize(stream);
                    break;
                case PacketType.SOCKET_CONNECTION_STATUS:
                    packet = (SocketConnectionStatus)formatter.Deserialize(stream);
                    break;
                case PacketType.SOCKET_STATUS:
                    packet = (SocketStatus)formatter.Deserialize(stream);
                    break;
                case PacketType.VIRTUAL_IP:
                    packet = (VirtualIP)formatter.Deserialize(stream);
                    break;
            }

            return packet;
        }
    }

    public static class PacketType
    {
        public const byte SOCKET_STATUS_REQUEST = 1;
        public const byte SOCKET_STATUS = 2;
        public const byte SOCKET_CONNECTION_REQUEST = 3;
        public const byte SOCKET_CONNECTION_STATUS = 4;
        public const byte ARBITRARY_DATA = 5;
        public const byte VIRTUAL_IP = 6;
        public const byte SOCKET_STATUS_RESPONSE = 7;
    }
}
