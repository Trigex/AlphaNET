using MongoDB.Driver;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WatsonTcp;

namespace AlphaServer
{
    public class AlphaServer
    {
        public const string CONNECTION_STRING = "mongodb://localhost:27017";
        public const string DB = "alphanet";

        static void Main(string[] args)
        {
            Database db = new Database();
            db.Init().Wait();

            WatsonTcpServer server = new WatsonTcpServer("127.0.0.1", 1337);
            server.ClientConnected = ClientConnected;
            server.ClientDisconnected = ClientDisconnected;
            server.MessageReceived = MessageReceived;
            server.Debug = false;
            server.Start();
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
            HandlePacket(data);
            return true;
        }

        static void HandlePacket(byte[] data)
        {

        }


    }
}
