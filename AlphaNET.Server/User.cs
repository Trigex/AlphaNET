namespace AlphaNET.Server
{
    public class User
    {
        public string IpAddress { get; set; } // the unique ID should be a user's real IP address
        public string VirtualIPAddress { get; set; }
    }
}
