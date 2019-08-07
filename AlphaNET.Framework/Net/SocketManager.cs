using AlphaNET.Framework.Net.Packets;
using System.Collections.Generic;

namespace AlphaNET.Framework.Net
{
    public class SocketManager
    {
        public List<Socket> Sockets { get; private set; }
        private ServerClient serverClient;

        public SocketManager(ServerClient serverClient)
        {
            this.serverClient = serverClient;
        }

        public NetStatusCode AddSocket(Socket socket)
        {
            if (Sockets.Count != 0)
            {
                foreach (Socket addedSocket in Sockets)
                {
                    if (addedSocket.Address.ToString() == socket.Address.ToString())
                    {
                        return NetStatusCode.AddressInUse;
                    }
                }

                Sockets.Add(socket);
                return NetStatusCode.AddedSocket;
            }
            else
            {
                Sockets.Add(socket);
                return NetStatusCode.AddedSocket;
            }
        }

        public void ConnectSocketToEndpoint(Socket socket)
        {
            Address endpoint = socket.EndpointAddress;
            Address client = socket.Address;
            // Send RequestSocketStatus to server
            serverClient.Send(new RequestSocketConnection(endpoint, client));
        }
    }
}
