using AlphaNET.Framework.Net.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlphaNET.Framework.Net
{
    public class SocketManager
    {
        private TcpClient _tcpClient;
        private List<Socket> _socketList;
        private ManualResetEvent _mre = new ManualResetEvent(false);

        public SocketManager(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _socketList = new List<Socket>();
        }

        public NetStatusCode BindSocket(Socket socket, Address address)
        {
            if(FindSocketByBindedAddress(address) == null) // Address isn't in use
            {
                socket.LocalAddress = address;
                _socketList.Add(socket);
                return NetStatusCode.SocketBinded;
            } else // Address in use
            {
                return NetStatusCode.AddressInUse;
            }
        }

        public NetStatusCode ListenOnSocket(Socket socket)
        {
            if (FindSocketByBindedAddress(socket.LocalAddress) != null)
            {
                socket.Listening = true;
                return NetStatusCode.SocketListening;
            } else
            {
                return NetStatusCode.SocketNotBinded;
            }
        }

        public NetStatusCode AcceptOnSocket(Socket socket)
        {
            if(FindSocketByBindedAddress(socket.LocalAddress) != null)
            {
                if(FindSocketByBindedAddress(socket.LocalAddress).Listening)
                {
                    return NetStatusCode.AddressInUse;
                } else
                {
                    return NetStatusCode.SocketNotListening;
                }
            } else
            {
                return NetStatusCode.SocketNotBinded;
            }
        }

        public NetStatusCode ConnectSocket(Socket socket, Address destinationAddress, ushort localPort = 0)
        {
            if (socket.LocalAddress == null || socket.LocalAddress.IpAddress == null || socket.LocalAddress.Port == 0) // Assosiate the socket with local address
            {
                ushort port;

                if (localPort != 0)
                    port = localPort;
                else
                    port = 24;

                socket.LocalAddress = new Address(_tcpClient.VirtualIP.ip, port);
            }

            socket.EndpointAddress = destinationAddress;

            SocketStatus status = null;
            _tcpClient.SocketStatusRecieved += (socketStatus) =>
            {
                status = socketStatus;
                _mre.Set();
            };

            // send socket status request
            _tcpClient.Send(new SocketStatusRequest(socket.LocalAddress, socket.EndpointAddress));
            // wait for a response
            
            _mre.WaitOne();

            if(status!=null)
            {
                if(status.Listening) // destination is open
                {
                    _tcpClient.Send(new SocketConnectionRequest(socket.LocalAddress, socket.EndpointAddress)); // attempt to connect
                    return NetStatusCode.SocketConnected;
                } else
                {
                    return NetStatusCode.SocketNotListening;
                }
            } else
            {
                return NetStatusCode.InvalidAddress;
            }
        }

        public SocketStatusResponse OnSocketStatusRequested(SocketStatusRequest reqSocketStatus)
        {
            var sockQuery = FindSocketByBindedAddress(reqSocketStatus.DestinationAddress);
            if (sockQuery != null)
            {
                return new SocketStatusResponse(new SocketStatus(true, true), reqSocketStatus.DestinationAddress);
            } else
            {
                Console.WriteLine("A local socket matching the requested socket doesn't exist");
                return new SocketStatusResponse(new SocketStatus(false, false), reqSocketStatus.DestinationAddress);
            }
        }

        public string GetIpAddress()
        {
            return _tcpClient.VirtualIP.ip;
        }

        private Socket FindSocketByBindedAddress(Address address)
        {
            return _socketList.Where(s => s.LocalAddress.IpAddress == address.IpAddress && s.LocalAddress.Port == address.Port).SingleOrDefault();
        }
    }
}
