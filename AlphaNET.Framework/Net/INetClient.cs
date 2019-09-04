using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Net
{
    /// <summary>
    /// An interface describing functionality to interact with an AlphaNET Server. Useful for having a TcpClient for Windows/Linux/macOS clients,
    /// and a WebSocketClient for web clients.
    /// </summary>
    public interface INetClient
    {
        bool Connected { get; }
    }
}
