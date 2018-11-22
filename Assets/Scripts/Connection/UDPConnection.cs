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

    public bool Listening = true;

    protected UDPConnection(Action<IPAddress, int, byte[]> passPacket) {
        this.passPacket = passPacket;
    }

    public void Listen() {
        while (Listening) {
            try {
                IPEndPoint senderIp = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref senderIp);
                passPacket(senderIp.Address, senderIp.Port, data);
            } catch (ThreadAbortException abort) {
                throw abort;
            } catch (Exception err) {
                Debug.Log(err);
            }
        }
        
        Debug.Log("Ending Listening");
    }

    public void Destroy() {
        Listening = false;
        client.Close();
    }

    public virtual void SendPacket(byte[] data, IPAddress ip, int port) {
        Debug.Log("SENDING to " + ip);
        try {
            client.Send(data, data.Length, new IPEndPoint(ip, port));
        }
        catch (Exception err) {
            Debug.LogError(err);
        }
    }
}