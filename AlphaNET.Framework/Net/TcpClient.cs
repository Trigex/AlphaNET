using AlphaNET.Framework.Net.Packets;
using System.Diagnostics;
using WatsonTcp;

namespace AlphaNET.Framework.Net
{
    public class TcpClient
    {
        private const string IP = "127.0.0.1";
        private const int PORT = 1337;

        private WatsonTcpClient _client;
        public VirtualIP virtualIp { get; private set; }

        public TcpClient()
        {
            _client = new WatsonTcpClient(IP, PORT);
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
            Debug.WriteLine("Sent: " + packet.GetType());
            _client.Send(packet.ToBytes());
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
            Debug.WriteLine("Recieved: " + data.ToString());
            switch(data[0])
            {
                case PacketType.VIRTUAL_IP: // We recieved our Virtual IP from the server, set it
                    virtualIp = VirtualIP.FromBytes(data);
                    Debug.WriteLine(string.Format("VirtualIP: {0}", virtualIp.ip));
                    break;
                default:
                    Debug.WriteLine(string.Format("Unknown or incorrect context PacketType: {0}", data[0]));
                    break;
            }
            return true;
        }
    }
}
