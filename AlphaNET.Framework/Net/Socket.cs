namespace AlphaNET.Framework.Net
{
    /// <summary>
    /// <c>Socket</c> represents a virtual AlphaNET socket, which can be used to remotely send or recieve data with another <c>Socket</c>
    /// </summary>
    public class Socket
    {
        /// <summary>
        /// The local <c>Address</c> this <c>Socket</c> is operating on
        /// </summary>
        public Address LocalAddress { get; set; }
        /// <summary>
        /// The endpoint <c>Address</c> this <c>Socket</c> is connected to
        /// </summary>
        public Address EndpointAddress { get; set; }
        /// <summary>
        /// Is this Socket connected to another Socket?
        /// </summary>
        public bool Connected { get; set; }
        /// <summary>
        /// Is this Socket currently listening for incoming connections?
        /// </summary>
        public bool Listening { get; set; }

        public Socket()
        {
            Connected = false;
            Listening = false;
        }
    }
}
