# Networking Protocol

# Basic Design

All AlphaNET clients automatically connect to a central server on startup (The default being the official AlphaNET server instance, however, once can use a custom address via CLI arguments)
This Server (Implemented in `AlphaNET.Server`) is responsible for assigning all clients a virtual IP, used to identify a given AlphaNET client. This virtual IP is linked to the client's real IP, and stored in the database,
becoming that IP's permanent virtual IP. The Server, which either generates or retrieves a client IP's virtual IP on a new connection, then sends the client their virtual IP via the `VirtualIP` packet type. The client stores this
virtual IP in the `TcpClient` class, and is used in certain packet types. Clients, from here on out, can at will send any sort of packet to the server, and the server will deal with these packets accordingly.

The main purpose of this Server <--> Client connection however, is to allow proxied communication with other AlphaNET clients, through the use of Virtual Sockets. A protocol for the usage of these Virtual Sockets is defined below.

## Virtual Socket Protocol

## (NOTE: These implementation details are NOT solidified, and are subject to change) 

If a given client wants to open and listen on a local socket, no network communication is required. 

They instantiate a `Socket` object, which is added
to a `SocketManager` object instance. This `SocketManager` is directly responsible for handling packets received, and sending packets relating to sockets, through delegates provided by the `TcpClient`. The user does not directly interact with
the `SocketManager`.

If a given client wants to connect to a remote socket (A socket which has been opened, binded to an address, and is listening for connections), they create a local socket, and use the `socket.Connect(DestinationAddress)` method to connect.
The `SocketManager` then sends a `SocketStatusRequest` packet to the server, detailing the address that the socket is attempting to connect to. The server, upon receiving this packet, forwards it to the requested address' real IP, if such an IP was connected to the server.
In the event it's not, the server sends a `RequestFailed` packet back to the requesting client, detailing that the remote address was not online. In the event it was, and the `SocketStatusRequest` was successfully forwarded, the remote client's `SocketManager` 
checks if there is a socket binded to the request address locally. Based on the result of this check, a `SocketStatusResponse` packet is sent back to the server, detailing the state of the socket. The server forwards the response to the original requesting client.

If `SocketStatusResponse` indicated the remote address was able to be connected to, the `SocketManager` then sends a `SocketConnectionRequest` packet to the server, detailing the local and remote address of the two sockets to be connected.
The server, again checking if the remote client is actually connected, forwards this to said client. The remote client can then at will accept or deny the connection request. The remote client sends a `SocketConnectionResponse` packet to the server,
indicating if it has accepted the connection or not. If the connection was accepted, the server then adds the two clients to the `ActiveSockets` list, which it will check whenever data is sent between the two sockets.

From this point forward, the two sockets are then free to send `ArbitraryData` packets to one another, through usage of `socket.Send(byte[])`. When `socket.Send()` is used, the `SocketManager` formats the binary data properly, and sends it to the server.
The server checks to see if this `ArbitraryData` packet is associated with an active socket connection, and proxies it to the remote address.

(TODO: WRITE THE SOCKET POLLING SCHEME DESCRIPTION, AND THE SERVER'S REMOVING OF INACTIVE SOCKET CONNECTIONS)

To close a socket connection, a socket simple calls the `socket.Close()` method. The `SocketManager` then sends a `SocketClosed` packet to the server, which removes entry in `ActiveSockets` relating to the two sockets, if such an entry exists.