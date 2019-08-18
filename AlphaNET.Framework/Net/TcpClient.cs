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
        private SocketManager _socketManager;
        public VirtualIP virtualIp { get; private set; }

        public TcpClient(string ip, int port)
        {
            _client = new WatsonTcpClient(ip, port);
            _client.ServerConnected = ServerConnected;
            _client.ServerDisconnected = ServerDisconnected;
            _client.MessageReceived = MessageRecieved;
        }

        public void AddSocketManager(SocketManager socketManager)
        {
            _socketManager = socketManager;
        }

        public void Start()
        {
            _client.Start();
        }

        public void Send(Packet packet)
        {
            _client.Send(PacketUtils.Serialize(packet)); // send
            Console.WriteLine("Sent: " + packet.GetType());
        }

        private bool ServerConnected()
        {
            Console.WriteLine("Connected to server");
            return true;
        }

        private bool ServerDisconnected()
        {
            Console.WriteLine("Disconnected from server");
            return true;
        }

        private bool MessageRecieved(byte[] data)
        {
            Packet packet = PacketUtils.Deserialize(data);
            Console.WriteLine("Recieved: " + packet.GetType());
            try
            {
                switch (packet)
                {
                    case VirtualIP vip: // We recieved our Virtual IP from the server, set it
                        virtualIp = (VirtualIP)packet;
                        Console.WriteLine(string.Format("VirtualIP: {0}", virtualIp.ip));
                        break;
                    case SocketStatusRequest rss: // Recieved SocketStatusRequest from the server. Send a response!
                        var reqSocketStatus = (SocketStatusRequest)packet;
                        // pass to SocketManager
                        Console.WriteLine(string.Format("SocketStatusRequest: {0}", reqSocketStatus.requestedAddress.ToString()));
                        var resSocketStatus = _socketManager.OnSocketStatusRequested(reqSocketStatus);
                        Send(resSocketStatus);
                        break;
                    case SocketStatusResponse rssr:
                        var sockstatusres = (SocketStatusResponse)packet;
                        _socketManager.OnSocketStatusRetrieved(sockstatusres.SocketStatus);
                        break;
                    default:
                        Console.WriteLine(string.Format("Unknown or incorrect context PacketType: {0}", data[0]));
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }
    }
}
