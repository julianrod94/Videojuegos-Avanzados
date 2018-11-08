using System;
using System.Collections.Generic;
using System.Net;
using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using UnityEngine;

public class ServerConnectionManager : MonoBehaviour {
    private struct Connection {
        private IPAddress ip;
        private int port;

        public Connection(IPAddress ip, int port) {
            this.ip = ip;
            this.port = port;
        }

        public IPAddress Ip {
            get { return ip; }
        }

        public int Port {
            get { return port; }
        }
    }

    public static ServerConnectionManager Instance;
    
    public int Port;
    private Server server;
    private List<Action<ChannelManager>> initializers = new List<Action<ChannelManager>>();
    
    private readonly Dictionary<Connection, ChannelManager> _clients = new Dictionary<Connection, ChannelManager>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            server = new Server(sendToChannel);
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        server.SetupServer(Port);
    }

    public void AddInitializer(Action<ChannelManager> initializer) {
        initializers.Add(initializer);
    }

    public void sendToChannel(IPAddress ip, int port, byte[] packet) {
        Connection connection = new Connection(ip, port);
        if (!_clients.ContainsKey(connection)) addNewClient(connection);
        _clients[connection].ReceivePacket(packet);
    }

    private void addNewClient(Connection connection) {
        ChannelManager newCM = new ChannelManager(server, connection.Ip, connection.Port);
        Debug.LogWarning(initializers.Count);
        initializers.ForEach((a) => a(newCM));
        _clients.Add(connection, newCM);
    }
}