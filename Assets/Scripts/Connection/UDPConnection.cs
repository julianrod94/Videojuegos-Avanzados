using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public abstract class UDPConnection {
    public int LocalPort;
    Thread receiveThread;
    private Action<IPAddress, int, byte[]> passPacket;
    protected UdpClient client;

    protected UDPConnection(Action<IPAddress, int, byte[]> passPacket) {
        this.passPacket = passPacket;
    }

    public void Listen() {
        while (true) {
            try {
                IPEndPoint senderIp = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref senderIp);
                Debug.Log("PACKET RECEIVED" + senderIp.Address);
                passPacket(senderIp.Address, senderIp.Port, data);
            }
            catch (Exception err) {
                Debug.Log(err);
            }
        }
    }

    public void SendPacket(byte[] data, IPAddress ip, int port) {
        Debug.Log("SENDING to " + ip);
        try {
            client.Send(data, data.Length, new IPEndPoint(ip, port));
        }
        catch (Exception err) {
            Debug.LogError(err);
        }
    }
}