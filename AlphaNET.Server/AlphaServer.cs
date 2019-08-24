using NLog;
using System.IO;

namespace AlphaNET.Server
{
    public static class AlphaServer
    {
        /// <summary>
        /// Main TcpServer instance, through which the server communicates with clients
        /// </summary>
        private static TcpServer _server;
        /// <summary>
        /// Static configuration file used throughout the program 
        /// </summary>
        public static readonly Config Config = Config.CreateConfig(File.ReadAllText("server.json"));

        public static bool Running = false;

        private static void Main(string[] args)
        {
            // Load nlog configuration file
            LogManager.LoadConfiguration("nlog.config");
            // AlphaServer's logger instance
            var logger = LogManager.GetCurrentClassLogger();
            
            logger.Info("Server is starting...");
            
            _server = new TcpServer();
            // The TcpServer encountered an error during initialization (details of the error appear in it's class logger)
            if (_server.Init(Config.Tcp.Ip, Config.Tcp.Port) != true)
            {
                logger.Fatal("Tcp Server unsuccessfully initialized, exiting...");
                return;
            }
            else
                Running = true;

            while (Running)
            {
            }
            
            logger.Info("The server is shutting down! Broke out of main loop");
        }
    }
}
