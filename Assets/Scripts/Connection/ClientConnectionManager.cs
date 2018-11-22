using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ChannelNS;
using SenderStrategyNS;
using ServerNS;
using UnityEngine;
using Utils;

public class ClientConnectionManager : MonoBehaviour {
    public string ServerIp;
    public int ServerPort;
    private Client client;
    public ChannelManager ChannelManager;

    private Thread listenThread;

    public static ClientConnectionManager Instance;

    [Range(0,100)]
    public float packetLoss = 0;
    
    [Range(0,1000)]
    public float Latency = 0;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            client = new Client(sendToChannel);
            ChannelManager = new ChannelManager(client, IPAddress.Parse(ServerIp), ServerPort);
            try {
                client.Connect(ServerIp, ServerPort);
                listenThread = new Thread(client.Listen);
                listenThread.IsBackground = true;
                listenThread.Start();
            } catch (Exception e) {
                Debug.LogError(e);
            }
            Debug.Log("SUCCESFULLY OPEN PORT: " + client.LocalPort);
        } else {
            Destroy(this);
        }
    }

    private void Update() {
        CurrentTime.Time = Time.time;
    }

    public void sendToChannel(IPAddress ip, int port, byte[] packet) {
        ChannelManager.ReceivePacket(packet);
    }

    private void OnDestroy() {
        client.Destroy();
    }
}