using NLog;

namespace AlphaNET.Server
{
    public class AlphaServer
    {
        public const string IP = "127.0.0.1";
        public const int PORT = 1337;
        public static TcpServer Server;

        static void Main(string[] args)
        {
            bool running = false;

            LogManager.LoadConfiguration("nlog.config");
            var log = LogManager.GetCurrentClassLogger();
            log.Info("Server is starting...");
            Server = new TcpServer();
            if (Server.Init(IP, PORT) != true)
            {
                log.Fatal("Tcp Server unsuccessfully initialized, exiting...");
                return;
            }
            else
            {
                running = true;
            }

            log.Info("Entering main loop...");
            while (running)
            {
            }

            log.Info("The server is shutting down! Broke out of main loop");
        }
    }
}
