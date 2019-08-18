using AlphaNET.Framework.Net;
using AlphaNET.Framework.Net.Packets;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
                _logger.Info(string.Format("Tcp Server started! Listening at {0}:{1}", ip, port));
            }
            catch (Exception e)
            {
                _logger.Error(string.Format("Unable to initialize TcpServer! \n\n{0}", e.Message));
                return false;
            }
            return true;
        }

        private bool ClientConnected(string ipPort)
        {
            // When a client initally connects, generate or find a virtual ip for their real ip address, and send it to them

            _logger.Info(string.Format("User Connected, IP {0}", ipPort));

            string virtualIp; // Virtual IP we will eventually send back to client
            string ip = Utils.StripIpPort(ipPort); // ipPort, but with the port stripped

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
                _logger.Info(string.Format("User's existing Virtual IP is: {0}", virtualIp));
            }
            else // New user, create entry
            {
                virtualIp = Utils.GenerateIpAddress();
                if (QueryUserContext.GetVirtualIPCount(virtualIp) > 0) // Make sure this IP isn't a duplicate
                {
                    _logger.Warn("Duplicate Virtual IP!");
                    ClientConnected(ipPort); // Rerun this method, generate another
                }

                QueryUserContext.AddUser(new User { RealIp = ip, VirtualIp = virtualIp });
                _logger.Info(string.Format("Generated a Virtual IP for a new user: {0}", virtualIp));
            }

            // Send client their virtual IP
            VirtualIP virtualIpPacket = new VirtualIP(virtualIp);
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
                    case SocketStatusRequest rss: // Source is requesting packet SocketStatus of the Destination
                        var SocketStatusRequest = (SocketStatusRequest)packet;
                        _logger.Info(string.Format("SocketStatusRequest: For address {0}", SocketStatusRequest.SourceAddress.ToString()));
                        var destinationUser = GetConnectedUserByVirtual(SocketStatusRequest.SourceAddress.IpAddress);
                        // Check if the client of the remote address is connected to this server
                        if (destinationUser != null)
                        {
                            // Add this request to ongoingrequests
                            _ongoingRequests.Add(new OngoingRequest(SocketStatusRequest.DestinationAddress, GetConnectedUserByReal(ipPort), SocketStatusRequest.SourceAddress, destinationUser, PacketType.SOCKET_STATUS));
                            // Ask the destination for packet SocketStatus
                            Send(SocketStatusRequest, destinationUser.RealIp);
                        }
                        else
                        {
                            // destination not connected, send our own SocketStatusResponse to the requesting client
                            _logger.Info(string.Format("The requested address isn't connected..."));
                            Send(new SocketStatusResponse(new SocketStatus(false, false), null), ipPort);
                        }
                        break;
                    case SocketStatusResponse rssr: // destination is responding to a SocketStatusRequest
                        var SocketStatusRequestResp = (SocketStatusResponse)packet;
                        var ongoingRequest = _ongoingRequests.Where(r => r.DestinationUser == GetConnectedUserByReal(ipPort) && r.RequestedPacketType == PacketType.SOCKET_STATUS).SingleOrDefault();
                        _logger.Info(string.Format("SocketStatusResponse: To address {0}", SocketStatusRequestResp.DestinationAddress.ToString()));
                        if(ongoingRequest != null) // Check if there's actually an ongoingrequest
                        {
                            // send to the original request user
                            Send(SocketStatusRequestResp.SocketStatus, ongoingRequest.SourceUser.RealIp);
                            // remove from ongoing
                            _ongoingRequests.Remove(ongoingRequest);
                        }
                        break;
                    case SocketConnectionRequest rsc: // source is requesting connection to destination
                        var requestSocketConn = (SocketConnectionRequest)packet;
                        _logger.Info(string.Format("SocketConnectionRequest: Remote: {0}, Requesting: {1}", requestSocketConn.SourceAddress.ToString(), requestSocketConn.DestinationAddress.ToString()));
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
                        var socketConnStatus = (SocketConnectionStatus)packet;
                        _logger.Info(string.Format("SocketConnectionStatus: Connected: {0}", socketConnStatus.Connected));
                        break;
                    default:
                        _logger.Warn(string.Format("Unknown or incorrect context PacketType: {0}", data[0]));
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
            return _connectedUsers.Where(u => u.RealIp == realIp).SingleOrDefault();
        }

        private User GetConnectedUserByVirtual(string virtualIp)
        {
            return _connectedUsers.Where(u => u.VirtualIp == virtualIp).SingleOrDefault();
        }

        private void RemoveConnectedUser(string realIp)
        {
            var index = _connectedUsers.FindIndex(u => u.RealIp == realIp);
            if(index >= 0)
            {
                _connectedUsers.RemoveAt(index);
                _logger.Info(String.Format("Removed user {0} from connected users list", realIp));
            }
        }

        private void AddUserToConnected(User user)
        {
            _logger.Info(String.Format("Added user {0} to connected users list", user.RealIp));
            _connectedUsers.Add(user);
        }
    }

    public class OngoingRequest
    {
        public Address SourceAddress;
        public User SourceUser;
        public Address DestinationAddress;
        public User DestinationUser;
        public byte RequestedPacketType; // The packet type the chain wants to send to source as the end goal

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
