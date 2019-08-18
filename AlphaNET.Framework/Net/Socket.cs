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
        public Address Address { get; set; }
        /// <summary>
        /// The endpoint <c>Address</c> this <c>Socket</c> is connected to
        /// </summary>
        public Address EndpointAddress { get; set; }
        /// <summary>
        /// Is this Socket connected to another Socket?
        /// </summary>
        public bool Connected { get; private set; }
        /// <summary>
        /// Is this Socket currently listening for incoming connections?
        /// </summary>
        public bool Listening { get; private set; }
        /// <summary>
        /// The <c>ProtocolType</c> this socket is using
        /// </summary>
        public SocketManager socketManager { get; private set; }

        public Socket(Address localAddress)
        {
            Address = localAddress;
            Connected = false;
            Listening = false;
        }

        public void Bind()
        {

        }

        public void Listen()
        {

        }

        public NetStatusCode Connect(Address endpointAddress)
        {
            if (Connected == false && Listening == false && socketManager != null && endpointAddress != null)
            {
                EndpointAddress = endpointAddress;
                // SocketManager will send a RequestSocketConnect packet to AlphaNET.Server, and the AlphaNET server will send a SocketStatusRequest packet to the remote address.
                // AlphaNET server will relay the socket status to us, or send AddressNotFound if the remote address is not connected
                return NetStatusCode.SocketConnected;
            }
            else
            {
                return NetStatusCode.NoSocketManager;
            }
        }

        /// <summary>
        /// Blocks, as it listens for connections
        /// </summary>
        public void Accept()
        {

        }

        /// <summary>
        /// Unbind a free resources related to the socket
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Send bytes to connected Server socket
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {

        }

        public void Recieve(byte[] bytes)
        {

        }
    }
}
