using AlphaNET.Framework.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Proxies
{
    class SocketManagerProxy
    {
        private SocketManager _socketManager;

        public SocketManagerProxy(SocketManager socketManager)
        {
            _socketManager = socketManager;
        }

        public NetStatusCode BindSocket(Socket socket, Address address)
        {
            return _socketManager.BindSocket(socket, address);
        }

        public NetStatusCode ListenOnSocket(Socket socket)
        {
            return _socketManager.ListenOnSocket(socket);
        }

        public NetStatusCode AcceptOnSocket(Socket socket)
        {
            return _socketManager.AcceptOnSocket(socket);
        }

        public NetStatusCode ConnectSocket(Socket socket, Address destinationAddress, ushort localPort = 0)
        {
            return _socketManager.ConnectSocket(socket, destinationAddress, localPort);
        }

        public string GetIpAddress()
        {
            return _socketManager.GetIpAddress();
        }
    }
}
