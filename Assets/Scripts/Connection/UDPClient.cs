using System;
using System.Net;
using System.Net.Sockets;

public class Client : UDPConnection {
    public Client(Action<IPAddress, int, byte[]> passPacket) : base(passPacket) {
    }

    public void Connect(string ServerIp, int ServerPort) {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
        client = new UdpClient();
    }
    
}