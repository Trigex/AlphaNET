/// <reference path="kernel/kernel.d.ts" />
function net() {
    if(Global.ProcessArguments[1]) {
        switch(Global.ProcessArguments[1]) {
            case "client":
                // open a socket
                let socket = new Socket(new Address(SocketManager._tcpClient.virtualIp.ip, 8020));
                socket.EndpointAddress = new Address("11.1", 80);
                SocketManager.ConnectSocketToEndpoint(socket);
                break;
            case "server":
                break;
        }
    } else {

    }
}

net();