using AlphaNET.Framework.Net.Packets;

namespace AlphaNET.Framework.Net
{
    public class SocketManager
    {
        public TcpClient _tcpClient; // temp public for testing

        public SocketManager(TcpClient TcpClient)
        {
            _tcpClient = TcpClient;
        }

        public void ConnectSocketToEndpoint(Socket socket)
        {
            Address endpoint = socket.EndpointAddress;
            Address client = socket.Address;
            // Send RequestSocketStatus to server
            _tcpClient.Send(new RequestSocketStatus(endpoint, client));
        }
    }
}
