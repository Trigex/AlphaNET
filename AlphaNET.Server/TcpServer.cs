using AlphaNET.Framework.Net.Packets;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WatsonTcp;

namespace AlphaNET.Server
{
    public class TcpServer
    {
        private WatsonTcpServer _server;
        private List<User> _connectedUsers;
        private Logger _logger;

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
            if (GetUserInConnectedList(ip) != null)
            {
                _server.DisconnectClient(ipPort);
            }

            using (var db = new UserContext()) // open db context
            {
                // check if user already exists in database
                var userDbEntry = db.Users.Where(u => u.RealIp == ip).FirstOrDefault();
                if(userDbEntry!=null) // User exists
                {
                    _logger.Info("Client is an existing user!");
                    virtualIp = userDbEntry.VirtualIp;
                    _logger.Info(string.Format("Client's existing Virtual IP is: {0}", virtualIp));
                } else // New user, create entry
                {
                    _logger.Info("Client is a new user!");
                    virtualIp = Utils.GenerateIpAddress();
                    if (db.Users.Where(u => u.VirtualIp == virtualIp).ToList().Count > 0) // Make sure this IP isn't a duplicate
                    {
                        _logger.Warn("Duplicate Virtual IP!");
                        ClientConnected(ipPort); // Rerun this method, generate another
                    }

                    var user = new User { RealIp = ip, VirtualIp = virtualIp };
                    db.Users.Add(user);
                    db.SaveChanges();
                    _logger.Info(string.Format("Generated new Virtual IP for client: {0}", virtualIp));
                }
            }

            // Send client their virtual IP
            VirtualIP virtualIpPacket = new VirtualIP(virtualIp);
            _server.Send(ipPort, virtualIpPacket.ToBytes());
            // Add them to connectedUsers list
            _connectedUsers.Add(new User { RealIp = ip, VirtualIp = virtualIp });
            _logger.Info("Sent client their Virtual IP, and added them to the Connected Users list");
            return true;
        }

        private bool ClientDisconnected(string ipPort)
        {
            RemoveUserFromConnectedList(Utils.StripIpPort(ipPort));
            _logger.Info("Client disconnected: " + ipPort);
            return true;
        }

        private bool MessageReceived(string ipPort, byte[] data)
        {
            switch (data[0]) // First byte should always be the PacketTypeCode
            {
                case PacketType.REQUEST_SOCKET_STATUS: // Client is requesting packet SocketStatus of the given address
                    var requestSocketStatus = RequestSocketStatus.FromBytes(data);
                    _logger.Info(string.Format("RequestSocketStatus: For address {0}", requestSocketStatus.remoteAddress.ToString()));
                    // Check if the client of the address exists
                    // Check if the client of the address is connected to this server
                    // Ask the client of the address for packet SocketStatus
                    // Send SocketStatus back to requesting client
                    break;
                case PacketType.SOCKET_STATUS:
                    var socketStatus = SocketStatus.FromBytes(data);
                    _logger.Info(string.Format("SocketStatus: Open: {0}, Listening: {1}", socketStatus.Open, socketStatus.Listening));
                    break;
                case PacketType.REQUEST_SOCKET_CONNECTION:
                    var requestSocketConn = RequestSocketConnection.FromBytes(data);
                    Debug.WriteLine(requestSocketConn);
                    _logger.Info(string.Format("RequestSocketConnection: Remote: {0}, Requesting: {1}", requestSocketConn.remoteAddress.ToString(), requestSocketConn.requestingAddress.ToString()));
                    break;
                case PacketType.SOCKET_CONNECTION_STATUS:
                    var socketConnStatus = SocketConnectionStatus.FromBytes(data);
                    _logger.Info(string.Format("SocketConnectionStatus: Connected: {0}", socketConnStatus.Connected));
                    break;
                default:
                    _logger.Warn(string.Format("Unknown or incorrect context PacketType: {0}", data[0]));
                    break;
            }

            return true;
        }

        private User GetUserInConnectedList(string realIp)
        {
            return _connectedUsers.Where(u => u.RealIp == realIp).SingleOrDefault();
        }

        private void RemoveUserFromConnectedList(string realIp)
        {
            var index = _connectedUsers.FindIndex(u => u.RealIp == realIp);
            _connectedUsers.RemoveAt(index);
        }
    }
}
