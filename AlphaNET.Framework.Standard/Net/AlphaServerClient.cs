using System;
using System.Collections.Generic;
using System.Text;
using WatsonTcp;

namespace AlphaNET.Framework.Standard.Net
{
    public class AlphaServerClient
    {
        const string IP = "127.0.0.1";
        const int PORT = 1337;

        private WatsonTcpClient _client;

        public AlphaServerClient()
        {
            _client = new WatsonTcpClient(IP, PORT);
            _client.ServerConnected = ServerConnected;
            _client.ServerDisconnected = ServerDisconnected;
            _client.MessageReceived = MessageRecieved;
        }

        public void Send()
        {

        }

        private bool ServerConnected()
        {
            Console.WriteLine("Connected to server");
            return true;
        }

        private bool ServerDisconnected()
        {
            Console.WriteLine("Server disconnected");
            return true;
        }

        private bool MessageRecieved(byte[] data)
        {
            return true;
        }
    }
}
