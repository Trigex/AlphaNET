using AlphaNET.Framework.Net.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlphaNET.Framework.Net
{
    public class SocketManager
    {
        public TcpClient _tcpClient; // temp public for testing
        private List<Socket> _socketList;

        public SocketManager(TcpClient TcpClient)
        {
            _tcpClient = TcpClient;
            _socketList = new List<Socket>();
        }

        public void ConnectSocketToEndpoint(Socket socket)
        {
            Address endpoint = socket.EndpointAddress;
            Address client = socket.Address;
            // Send RequestSocketStatus to server
            _tcpClient.Send(new RequestSocketStatus(endpoint, client));
        }

        public void ListenOnSocket(Socket socket)
        {
            _socketList.Add(socket);
        }

        public RequestSocketStatusResponse OnSocketStatusRequested(RequestSocketStatus reqSocketStatus)
        {
            var sockQuery = _socketList.Where(s => s.Address.IpAddress == reqSocketStatus.requestedAddress.IpAddress && s.Address.Port == reqSocketStatus.requestedAddress.Port).SingleOrDefault();
            if (sockQuery != null)
            {
                return new RequestSocketStatusResponse(new SocketStatus(true, true), reqSocketStatus.requestingAddress);
            } else
            {
                Console.WriteLine("A local socket matching the requested socket doesn't exist")
                {
                    return new RequestSocketStatusResponse(new SocketStatus(false, false), reqSocketStatus.requestingAddress);
                }
            }
        }
    }
}
