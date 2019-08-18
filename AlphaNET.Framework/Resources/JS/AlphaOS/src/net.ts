/// <reference path="kernel/types/os.d.ts" />
/// <reference path="kernel/system.ts" />
function net(args: string[]) {
    switch(args[0]) {
        case "test":
            // bind a socket
            let socket: Socket = new Socket();
            let address: Address = new Address(SocketManager.GetIpAddress(), 80);
            let binded: NetStatusCode = SocketManager.BindSocket(socket, address);
            Terminal.WriteLine(binded.toString());
            let socket2: Socket = new Socket();
            let connected = SocketManager.ConnectSocket(socket2, address, 29);
            break;
    }
}

net(args);