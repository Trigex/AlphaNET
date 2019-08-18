using NLog;
using System.IO;

namespace AlphaNET.Server
{
    public class AlphaServer
    {
        public static TcpServer Server;
        public static Config Config = Config.CreateConfig(File.ReadAllText("server.json"));

        private static void Main(string[] args)
        {
            var running = false;
            LogManager.LoadConfiguration("nlog.config");
            var log = LogManager.GetCurrentClassLogger();
            log.Info("Server is starting...");
            Server = new TcpServer();
            if (Server.Init(Config.Tcp.Ip, Config.Tcp.Port) != true)
            {
                log.Fatal("Tcp Server unsuccessfully initialized, exiting...");
                return;
            }
            else
                running = true;

            while (running)
            {
            }
            log.Info("The server is shutting down! Broke out of main loop");
        }
    }
}
