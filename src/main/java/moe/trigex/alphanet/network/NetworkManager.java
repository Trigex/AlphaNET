package moe.trigex.alphanet.network;

import java.io.IOException;
import java.net.Socket;

public class NetworkManager {
    private Socket sock;

    public NetworkManager() {
        try {
            sock = new Socket("localhost", 8020);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
