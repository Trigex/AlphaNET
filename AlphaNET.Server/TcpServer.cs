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

        public TcpServer()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool Init(string ip, int port)
        {
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
            _logger.Info("Client connected: " + ipPort);

            string virtualIp; // Virtual IP we will eventually send back to client
            string ip = Utils.StripIpPort(ipPort); // ipPort, but with the port stripped

            // TODO: Move all DB query stuff into methods on UserContext

            // 1 client at a time per ip ;) At some point I'll implement a system to allow a realIp to own more than one VirtualIP.
            // But gotta find a way to regulate that, maybe a client is given X number of allowed virtual IPs?
            if (GetUserInConnectedList(ipPort) != null)
            {
                _server.DisconnectClient(ipPort);
            }
            // check if user already exists in database
            var userDbEntry = QueryUserContext.GetUser(ip);

            if (userDbEntry != null) // User exists
            {
                _logger.Info("Client is an existing user!");
                virtualIp = userDbEntry.VirtualIp;
                _logger.Info(string.Format("Client's existing Virtual IP is: {0}", virtualIp));
            }
            else // New user, create entry
            {
                _logger.Info("Client is a new user!");
                virtualIp = Utils.GenerateIpAddress();
                if (QueryUserContext.GetVirtualIPCount(virtualIp) > 0) // Make sure this IP isn't a duplicate
                {
                    _logger.Warn("Duplicate Virtual IP!");
                    ClientConnected(ipPort); // Rerun this method, generate another
                }

                var user = new User { RealIp = ip, VirtualIp = virtualIp };
                QueryUserContext.AddUser(user);
                _logger.Info(string.Format("Generated new Virtual IP for client: {0}", virtualIp));
            }

            // Send client their virtual IP
            VirtualIP virtualIpPacket = new VirtualIP(virtualIp);
            Send(virtualIpPacket, ipPort);
            // Add them to connectedUsers list
            _connectedUsers.Add(new User { RealIp = ipPort, VirtualIp = virtualIp });
            _logger.Info("Sent client their Virtual IP, and added them to the Connected Users list");
            return true;
        }

        private bool ClientDisconnected(string ipPort)
        {
            RemoveUserFromConnectedList(Utils.StripIpPort(ipPort));
            _logger.Info("Client disconnected: " + ipPort);
            return true;
        }

        private void Send(Packet packet, string ipPort) // Just copied from TcpClient lol
        {
            byte[] packetData; // final buffer which will hold the data to send
            // Serialize packet, store in MemoryStream
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, packet);
            // create list from stream
            var dataList = new List<byte>(stream.ToArray());
            stream.Close(); // close
            dataList.Insert(0, packet.Type); // set first byte to packet type
            packetData = dataList.ToArray(); // set final buffer to list array contents
            _server.Send(ipPort, packetData); // send
            Debug.WriteLine("Sent: " + packet.GetType());
        }

        private bool MessageReceived(string ipPort, byte[] data)
        {
            // strip packet type byte before inserting into memory stream
            var dataList = new List<byte>(data);
            dataList.RemoveAt(0);
            var stream = new MemoryStream(dataList.ToArray());
            var formatter = new BinaryFormatter();

            switch (data[0]) // First byte should always be the PacketTypeCode
            {
                case PacketType.REQUEST_SOCKET_STATUS: // Client is requesting packet SocketStatus of the given address
                    var requestSocketStatus = (RequestSocketStatus)formatter.Deserialize(stream);
                    _logger.Info(string.Format("RequestSocketStatus: For address {0}", requestSocketStatus.requestedAddress.ToString()));
                    var remoteClient = GetUserInConnectedListByVirtual(requestSocketStatus.requestedAddress.IpAddress);
                    // Check if the client of the remote address is connected to this server
                    if (remoteClient != null)
                    {
                        // Add this request to ongoingrequests
                        _ongoingRequests.Add(new OngoingRequest(requestSocketStatus.requestingAddress, GetUserInConnectedList(ipPort), requestSocketStatus.requestedAddress, remoteClient, PacketType.SOCKET_STATUS));
                        // Ask the client of the address for packet SocketStatus
                        Send(requestSocketStatus, remoteClient.RealIp);
                    }
                    else
                    {
                        // Client not connected
                    }
                    break;
                case PacketType.REQUEST_SOCKET_STATUS_RESPONSE:
                    var requestSocketStatusResp = (RequestSocketStatusResponse)formatter.Deserialize(stream);
                    _logger.Info(string.Format("RequestSocketStatusResponse: To address {0}", requestSocketStatusResp.RequestingAddress.ToString()));
                    if() // Check if there's actually an ongoingrequest
                    break;
                case PacketType.REQUEST_SOCKET_CONNECTION:
                    var requestSocketConn = (RequestSocketConnection)formatter.Deserialize(stream);
                    Debug.WriteLine(requestSocketConn);
                    _logger.Info(string.Format("RequestSocketConnection: Remote: {0}, Requesting: {1}", requestSocketConn.remoteAddress.ToString(), requestSocketConn.requestingAddress.ToString()));
                    break;
                case PacketType.SOCKET_CONNECTION_STATUS:
                    var socketConnStatus = (SocketConnectionStatus)formatter.Deserialize(stream);
                    _logger.Info(string.Format("SocketConnectionStatus: Connected: {0}", socketConnStatus.Connected));
                    break;
                default:
                    _logger.Warn(string.Format("Unknown or incorrect context PacketType: {0}", data[0]));
                    break;
            }

            stream.Close();
            return true;
        }

        private User GetUserInConnectedList(string realIp)
        {
            return _connectedUsers.Where(u => u.RealIp == realIp).SingleOrDefault();
        }

        private User GetUserInConnectedListByVirtual(string virtualIp)
        {
            return _connectedUsers.Where(u => u.VirtualIp == virtualIp).SingleOrDefault();
        }

        private void RemoveUserFromConnectedList(string realIp)
        {
            var index = _connectedUsers.FindIndex(u => u.RealIp == realIp);
            _connectedUsers.RemoveAt(index);
        }
    }

    public class OngoingRequest
    {
        public Address requestingAddress;
        public User requestingUser;
        public Address requestedAddress;
        public User requestedUser;
        public byte RequestedPacketType; // The packet type the chain wants to send to requesting as the end goal

        public OngoingRequest(Address requestingAddress, User requestingUser, Address requestedAddress, User requestedUser, byte requestedPacketType)
        {
            this.requestingAddress = requestingAddress;
            this.requestingUser = requestingUser;
            this.requestedAddress = requestedAddress;
            this.requestedUser = requestedUser;
            this.RequestedPacketType = requestedPacketType;
        }
    }
}
