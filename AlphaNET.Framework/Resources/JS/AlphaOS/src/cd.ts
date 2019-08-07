/// <reference path="kernel/kernel.d.ts" />
function cd() {
    var socket = new Socket(new Address("127.0.0.1", 127));
    socket.EndpointAddress = new Address("999.999.999", 80);
    SocketManager.ConnectSocketToEndpoint(socket);
}

cd();