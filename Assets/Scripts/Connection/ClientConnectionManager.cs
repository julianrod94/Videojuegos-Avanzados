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
    private ChannelManager _channelManager;


    private void Awake() {
        client = new Client(sendToChannel);
        _channelManager = new ChannelManager(client, IPAddress.Parse(ServerIp), ServerPort);
        _channelManager.RegisterChannel(new CubePositionStateChannel(new CubePositionProvider(), new TrivialStrategy(),
            1000));
        client.Connect(ServerIp, ServerPort);
    }

    public void sendToChannel(IPAddress ip, int port, byte[] packet) {
        _channelManager.ReceivePacket(packet);
    }
}