using System;
using System.Collections.Generic;
using System.Net;
using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using UnityEngine;

public class ClientConnectionManager : MonoBehaviour {
    public string ServerIp;
    public int ServerPort;
    private Client client;
    public ChannelManager ChannelManager;


    public static ClientConnectionManager Instance;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            client = new Client(sendToChannel);
            ChannelManager = new ChannelManager(client, IPAddress.Parse(ServerIp), ServerPort);
            Debug.Log("CONNECTING");
            try {
                client.Connect(ServerIp, ServerPort);
            } catch (Exception e) {
                Debug.LogError(e);
            }
            Debug.Log("SUCCESFUL  " + client.LocalPort);
        } else {
            Destroy(this);
        }
    }

    public void sendToChannel(IPAddress ip, int port, byte[] packet) {
        ChannelManager.ReceivePacket(packet);
    }
}