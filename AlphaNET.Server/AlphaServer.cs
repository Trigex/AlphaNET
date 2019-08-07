namespace AlphaNET.Server
{
    public class AlphaServer
    {
        public const string CONNECTION_STRING = "mongodb://localhost:27017";
        public const string DB = "alphanet";
        public const string IP = "127.0.0.1";
        public const int PORT = 1337;
        public static Database db;
        public static TcpServer server;

        static void Main(string[] args)
        {
            db = new Database(CONNECTION_STRING);
            server = new TcpServer(IP, PORT);

            while (true)
            {
                // ah
            }
        }
    }
}
