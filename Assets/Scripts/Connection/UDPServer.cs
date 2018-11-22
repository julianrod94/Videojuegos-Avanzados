using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Server : UDPConnection {
    private Thread listenThread;

    public Server(Action<IPAddress, int, byte[]> passPacket) : base(passPacket) {
        
    }

    public void SetupServer(int port) {
        client = new UdpClient(port);
        Debug.Log("Listening " + port);
        listenThread = new Thread(Listen);
        listenThread.IsBackground = true;
        listenThread.Start();
    }

    public void Destroy() {
        Listening = false;
    }
}