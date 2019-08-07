using AlphaNET.Framework.Net.Packets;
using System.Collections.Generic;

namespace AlphaNET.Framework.Net
{
    public class SocketManager
    {
        private TcpClient _tcpClient;

        public SocketManager(TcpClient TcpClient)
        {
            _tcpClient = TcpClient;
        }

        public void ConnectSocketToEndpoint(Socket socket)
        {
            Address endpoint = socket.EndpointAddress;
            Address client = socket.Address;
            // Send RequestSocketStatus to server
            _tcpClient.Send(new RequestSocketConnection(endpoint, client));
        }
    }
}
