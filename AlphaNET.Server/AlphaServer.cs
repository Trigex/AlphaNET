using NLog;
using System.IO;

namespace AlphaNET.Server
{
    public class AlphaServer
    {
        public static TcpServer Server;
        public static Config CONFIG = Config.CreateConfig(File.ReadAllText("server.json"));

        static void Main(string[] args)
        {
            bool running = false;
            LogManager.LoadConfiguration("nlog.config");
            var log = LogManager.GetCurrentClassLogger();
            log.Info("Server is starting...");
            Server = new TcpServer();
            if (Server.Init(CONFIG.tcp.ip, CONFIG.tcp.port) != true)
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
