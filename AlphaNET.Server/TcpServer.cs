using AlphaNET.Framework.Net.Packets;
using System;
using System.Diagnostics;
using WatsonTcp;

namespace AlphaNET.Server
{
    public class TcpServer
    {
        private WatsonTcpServer _server;

        public TcpServer(string ip, int port)
        {
            _server = new WatsonTcpServer(ip, port);
            _server.ClientConnected = ClientConnected;
            _server.ClientDisconnected = ClientDisconnected;
            _server.MessageReceived = MessageReceived;
            _server.Debug = false;
            _server.Start();
        }

        static bool ClientConnected(string ipPort)
        {
            Console.WriteLine("Client connected: " + ipPort);
            return true;
        }

        static bool ClientDisconnected(string ipPort)
        {
            Console.WriteLine("Client disconnected: " + ipPort);
            return true;
        }

        static bool MessageReceived(string ipPort, byte[] data)
        {
            switch (data[0]) // First byte should always be the PacketTypeCode
            {
                case PacketTypeCodes.REQUEST_SOCKET_STATUS: // Client is requesting packet SocketStatus of the given address
                    var requestSocketStatus = RequestSocketStatus.FromBytes(data);
                    Console.WriteLine(string.Format("RequestSocketStatus: For address {0}", requestSocketStatus.remoteAddress.ToString()));
                    // Check if the client of the address exists
                    // Check if the client of the address is connected to this server
                    // Ask the client of the address for packet SocketStatus
                    // Send SocketStatus back to requesting client
                    break;
                case PacketTypeCodes.SOCKET_STATUS:
                    var socketStatus = SocketStatus.FromBytes(data);
                    Console.WriteLine(string.Format("SocketStatus: Open: {0}, Listening: {1}", socketStatus.Open, socketStatus.Listening));
                    break;
                case PacketTypeCodes.REQUEST_SOCKET_CONNECTION:
                    var requestSocketConn = RequestSocketConnection.FromBytes(data);
                    Debug.WriteLine(requestSocketConn);
                    Console.WriteLine(string.Format("RequestSocketConnection: Remote: {0}, Requesting: {1}", requestSocketConn.remoteAddress.ToString(), requestSocketConn.requestingAddress.ToString()));
                    break;
                case PacketTypeCodes.SOCKET_CONNECTION_STATUS:
                    var socketConnStatus = SocketConnectionStatus.FromBytes(data);
                    Console.WriteLine(string.Format("SocketConnectionStatus: Connected: {0}", socketConnStatus.Connected));
                    break;
                default:
                    Console.WriteLine("Unknown Packet code!");
                    break;
            }

            return true;
        }
    }
}
