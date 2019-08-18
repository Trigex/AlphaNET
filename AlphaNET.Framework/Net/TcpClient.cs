using AlphaNET.Framework.Net.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WatsonTcp;

namespace AlphaNET.Framework.Net
{
    public class TcpClient
    {
        private WatsonTcpClient _client;
        private SocketManager _socketManager;
        public VirtualIP VirtualIP { get; private set; }

        public delegate void RecieveSocketStatus(SocketStatus socketStatus);
        public event RecieveSocketStatus SocketStatusRecieved;
        public delegate void RecieveSocketConnectionStatus(SocketConnectionStatus socketConnectionStatus);
        public event RecieveSocketConnectionStatus SocketConnectionStatusRecieved;

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
                        VirtualIP = (VirtualIP)packet;
                        Console.WriteLine(string.Format("VirtualIP: {0}", VirtualIP.ip));
                        break;
                    case SocketStatusRequest rss: // Recieved SocketStatusRequest from the server. Send a response!
                        var reqSocketStatus = (SocketStatusRequest)packet;
                        // pass to SocketManager
                        Console.WriteLine(string.Format("SocketStatusRequest: {0}", reqSocketStatus.SourceAddress.ToString()));
                        var resSocketStatus = _socketManager.OnSocketStatusRequested(reqSocketStatus);
                        Send(resSocketStatus);
                        break;
                    case SocketStatus ss:
                        var socketStatus = (SocketStatus)packet;
                        SocketStatusRecieved(socketStatus);
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
