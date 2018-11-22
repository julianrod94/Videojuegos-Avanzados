using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using Random = System.Random;

public class Client : UDPConnection {
    static Random rand = new Random();

    public Client(Action<IPAddress, int, byte[]> passPacket) : base(getSenderAction(passPacket)) { }

    public static Action<IPAddress, int, byte[]> getSenderAction(Action<IPAddress, int, byte[]> passPacket) {
        return ((ip, Port, packet) => {
            if (rand.NextDouble() * 100 < ClientConnectionManager.Instance.packetLoss) {
                return;
            }

        if (ClientConnectionManager.Instance.Latency > 0) {
            ThreadPool.QueueUserWorkItem(delegate {
                Thread.Sleep(TimeSpan.FromMilliseconds(ClientConnectionManager.Instance.Latency));
                try {
                    passPacket(ip, Port, packet);
                } catch (Exception err) {
                    Debug.LogError(err);
                }
            });
        } else {
            try {
                passPacket(ip, Port, packet);
            }
            catch (Exception err) {
                Debug.LogError(err);
            }
        }
                
            }
        );
    }
    
    public void Connect(string ServerIp, int ServerPort) {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
        client = new UdpClient();
    }
    
    public override void SendPacket(byte[] data, IPAddress ip, int port) {
        Debug.Log("SENDING to " + ip);
        if (rand.NextDouble() * 100 < ClientConnectionManager.Instance.packetLoss) {
            Debug.Log("Package Lost");
            return;
        }

        if (ClientConnectionManager.Instance.Latency > 0) {
            ThreadPool.QueueUserWorkItem(delegate {
                Thread.Sleep(TimeSpan.FromMilliseconds(ClientConnectionManager.Instance.Latency));
                try {
                    client.Send(data, data.Length, new IPEndPoint(ip, port));
                } catch (Exception err) {
                    Debug.LogError(err);
                }
            });
        } else {
            try {
                client.Send(data, data.Length, new IPEndPoint(ip, port));
            }
            catch (Exception err) {
                Debug.LogError(err);
            }
        }
        
    }
    
}