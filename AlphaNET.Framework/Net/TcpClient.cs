using AlphaNET.Framework.Net.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using WatsonTcp;

namespace AlphaNET.Framework.Net
{
    public class TcpClient
    {
        private WatsonTcpClient _client;
        public VirtualIP virtualIp { get; private set; }

        public TcpClient(string ip, int port)
        {
            _client = new WatsonTcpClient(ip, port);
            _client.ServerConnected = ServerConnected;
            _client.ServerDisconnected = ServerDisconnected;
            _client.MessageReceived = MessageRecieved;
        }

        public void Start()
        {
            _client.Start();
        }

        public void Send(Packet packet)
        {
            byte[] packetData; // final buffer which will hold the data to send
            // Serialize packet, store in MemoryStream
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, packet);
            // create list from stream
            var dataList = new List<byte>(stream.ToArray());
            stream.Close(); // close
            dataList.Insert(0, packet.Type); // set first byte to packet type
            packetData = dataList.ToArray(); // set final buffer to list array contents
            _client.Send(packetData); // send
            Debug.WriteLine("Sent: " + packet.GetType());
        }

        private bool ServerConnected()
        {
            Debug.WriteLine("Connected to server");
            return true;
        }

        private bool ServerDisconnected()
        {
            Debug.WriteLine("Server disconnected");
            return true;
        }

        private bool MessageRecieved(byte[] data)
        {
            // strip packet type byte before inserting into memory stream
            var dataList = new List<byte>(data);
            dataList.RemoveAt(0);
            var stream = new MemoryStream(dataList.ToArray());
            var formatter = new BinaryFormatter();
            Debug.WriteLine("Recieved: " + data.ToString());
            try
            {
                switch (data[0])
                {
                    case PacketType.VIRTUAL_IP: // We recieved our Virtual IP from the server, set it
                        virtualIp = (VirtualIP)formatter.Deserialize(stream);
                        Debug.WriteLine(string.Format("VirtualIP: {0}", virtualIp.ip));
                        break;
                    default:
                        Debug.WriteLine(string.Format("Unknown or incorrect context PacketType: {0}", data[0]));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            stream.Close();
            return true;
        }
    }
}
