using AlphaNET.Framework.Net;
using AlphaNET.Framework.Net.Packets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WatsonTcp;

namespace AlphaNET.Server
{
    public class TcpServer
    {
        private WatsonTcpServer _server;
        private List<User> _connectedUsers;
        private Logger _logger;
        private List<OngoingRequest> _ongoingRequests;

        public TcpServer() { }

        public bool Init(string ip, int port)
        {
            _logger = LogManager.GetCurrentClassLogger();

            try
            {
                _server = new WatsonTcpServer(ip, port);
                _connectedUsers = new List<User>();
                _ongoingRequests = new List<OngoingRequest>();

                _server.ClientConnected = ClientConnected;
                _server.ClientDisconnected = ClientDisconnected;
                _server.MessageReceived = MessageReceived;
                _server.Debug = false;
                _server.Start();
                _logger.Info($"Tcp Server started! Listening at {ip}:{port}");
            }
            catch (Exception e)
            {
                _logger.Fatal($"Unable to initialize TcpServer! \n\n{e.Message}");
                return false;
            }
            return true;
        }

        private bool ClientConnected(string ipPort)
        {
            // When a client initially connects, generate or find a virtual ip for their real ip address, and send it to them

            _logger.Info($"User Connected, IP {ipPort}");

            string virtualIp; // Virtual IP we will eventually send back to client
            var ip = Utils.StripIpPort(ipPort); // ipPort, but with the port stripped

            // 1 client at a time per ip ;) At some point I'll implement a system to allow a realIp to own more than one VirtualIP.
            // But gotta find a way to regulate that, maybe a client is given X number of allowed virtual IPs?
            if (GetConnectedUserByReal(ipPort) != null)
            {
                _server.DisconnectClient(ipPort);
            }
            // check if user already exists in database
            var userDbEntry = QueryUserContext.GetUser(ip);

            if (userDbEntry != null) // User exists
            {
                virtualIp = userDbEntry.VirtualIp;
                _logger.Info($"User's existing Virtual IP is: {virtualIp}");
            }
            else // New user, create entry
            {
                virtualIp = Utils.GenerateIpAddress();
                if (QueryUserContext.GetVirtualIpCount(virtualIp) > 0) // Make sure this IP isn't a duplicate
                {
                    _logger.Warn("Duplicate Virtual IP!");
                    ClientConnected(ipPort); // Rerun this method, generate another
                }

                QueryUserContext.AddUser(new User { RealIp = ip, VirtualIp = virtualIp });
                _logger.Info($"Generated a Virtual IP for a new user: {virtualIp}");
            }

            // Send client their virtual IP
            var virtualIpPacket = new VirtualIP(virtualIp);
            Send(virtualIpPacket, ipPort);
            // Add them to connectedUsers list
            AddUserToConnected(new User { RealIp = ipPort, VirtualIp = virtualIp });
            return true;
        }

        private bool ClientDisconnected(string ipPort)
        {
            RemoveConnectedUser(ipPort);
            _logger.Info("User disconnected: " + ipPort);
            return true;
        }

        private void Send(Packet packet, string ipPort)
        {
            var bytes = PacketUtils.Serialize(packet);
            _server.Send(ipPort, bytes); // send
            _logger.Info("Sent packet \"{0}\", to {1}, {2} bytes", packet.GetType(), ipPort, bytes.Length);
        }

        private bool MessageReceived(string ipPort, byte[] data)
        {
            Packet packet = PacketUtils.Deserialize(data);

            try
            {
                switch (packet) // First byte should always be the PacketTypeCode
                {
                    case SocketStatusRequest ssr: // Source is requesting packet SocketStatus of the Destination
                        var socketStatusRequest = ssr;
                        _logger.Info(string.Format("SocketStatusRequest: For address {0}", socketStatusRequest.SourceAddress.ToString()));
                        var destinationUser = GetConnectedUserByVirtual(socketStatusRequest.SourceAddress.IpAddress);
                        // Check if the client of the remote address is connected to this server
                        if (destinationUser != null)
                        {
                            // Add this request to ongoingrequests
                            _ongoingRequests.Add(new OngoingRequest(socketStatusRequest.DestinationAddress, GetConnectedUserByReal(ipPort), socketStatusRequest.SourceAddress, destinationUser, PacketType.SOCKET_STATUS));
                            // Ask the destination for packet SocketStatus
                            Send(socketStatusRequest, destinationUser.RealIp);
                        }
                        else
                        {
                            // destination not connected, send our own SocketStatusResponse to the requesting client
                            _logger.Info(string.Format("The requested address isn't connected..."));
                            Send(new SocketStatusResponse(new SocketStatus(false, false), null), ipPort);
                        }
                        break;
                    case SocketStatusResponse ssr: // destination is responding to a SocketStatusRequest
                        var socketStatusRequestResp = ssr;
                        var ongoingRequest = _ongoingRequests.SingleOrDefault(r => r.DestinationUser == GetConnectedUserByReal(ipPort) && r.RequestedPacketType == PacketType.SOCKET_STATUS);
                        _logger.Info(
                            $"SocketStatusResponse: To address {socketStatusRequestResp.DestinationAddress.ToString()}");
                        if(ongoingRequest != null) // Check if there's actually an ongoingrequest
                        {
                            // send to the original request user
                            Send(socketStatusRequestResp.SocketStatus, ongoingRequest.SourceUser.RealIp);
                            // remove from ongoing
                            _ongoingRequests.Remove(ongoingRequest);
                        }
                        break;
                    case SocketConnectionRequest scr: // source is requesting connection to destination
                        var requestSocketConn = scr;
                        _logger.Info(
                            $"SocketConnectionRequest: Remote: {requestSocketConn.SourceAddress.ToString()}, Requesting: {requestSocketConn.DestinationAddress.ToString()}");
                        if(GetConnectedUserByVirtual(requestSocketConn.DestinationAddress.IpAddress) != null)
                        {
                            var destUser = GetConnectedUserByVirtual(requestSocketConn.DestinationAddress.IpAddress);
                            // Add ongoing request
                            _ongoingRequests.Add(new OngoingRequest(requestSocketConn.SourceAddress, 
                                GetConnectedUserByReal(ipPort), 
                                requestSocketConn.DestinationAddress, 
                                GetConnectedUserByVirtual(requestSocketConn.DestinationAddress.IpAddress), 
                                PacketType.SOCKET_CONNECTION_STATUS));

                            Send(requestSocketConn, destUser.RealIp);

                        }
                        else // Destination client isn't connected
                        {
                            Send(new SocketConnectionStatus(false), ipPort); // send our own response indicating false
                        }
                        break;
                    case SocketConnectionStatus scs: // destination is responding to connection request
                        var socketConnStatus = scs;
                        _logger.Info($"SocketConnectionStatus: Connected: {socketConnStatus.Connected}");
                        break;
                    default:
                        _logger.Warn($"Unknown or incorrect context PacketType: {data[0]}");
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }

            return true;
        }

        private User GetConnectedUserByReal(string realIp)
        {
            return _connectedUsers.SingleOrDefault(u => u.RealIp == realIp);
        }

        private User GetConnectedUserByVirtual(string virtualIp)
        {
            return _connectedUsers.SingleOrDefault(u => u.VirtualIp == virtualIp);
        }

        private void RemoveConnectedUser(string realIp)
        {
            var index = _connectedUsers.FindIndex(u => u.RealIp == realIp);
            if(index >= 0)
            {
                _connectedUsers.RemoveAt(index);
                _logger.Info($"Removed user {realIp} from connected users list");
            }
        }

        private void AddUserToConnected(User user)
        {
            _logger.Info($"Added user {user.RealIp} to connected users list");
            _connectedUsers.Add(user);
        }
    }

    public class OngoingRequest
    {
        public Address SourceAddress { get; private set; }
        public User SourceUser { get; private set; }
        public Address DestinationAddress { get; private set; }
        public User DestinationUser { get; private set; }
        public byte RequestedPacketType { get; private set; } // The packet type the chain wants to send to source as the end goal

        public OngoingRequest(Address sourceAddress, User sourceUser, Address destinationAddress, User destinationUser, byte requestedPacketType)
        {
            SourceAddress = sourceAddress;
            SourceUser = sourceUser;
            DestinationAddress = destinationAddress;
            DestinationUser = destinationUser;
            RequestedPacketType = requestedPacketType;
        }
    }
}
