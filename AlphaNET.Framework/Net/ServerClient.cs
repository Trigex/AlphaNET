using AlphaNET.Framework.Net.Packets;
using System.Diagnostics;
using WatsonTcp;

namespace AlphaNET.Framework.Net
{
    public class ServerClient
    {
        const string IP = "127.0.0.1";
        const int PORT = 1337;

        private WatsonTcpClient _client;

        public ServerClient()
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
            return true;
        }
    }
}
