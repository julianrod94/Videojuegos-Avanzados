using System.Collections.Generic;
using System.Net;
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

    public int Port;
    private Server server;
    private readonly Dictionary<Connection, ChannelManager> _clients = new Dictionary<Connection, ChannelManager>();

    private void Awake() {
        server = new Server(sendToChannel);
        server.SetupServer(Port);
    }

    public void sendToChannel(IPAddress ip, int port, byte[] packet) {
        Connection connection = new Connection(ip, port);
        if (!_clients.ContainsKey(connection)) addNewClient(connection);
        _clients[connection].ReceivePacket(packet);
    }

    private void addNewClient(Connection connection) {
        ChannelManager newCM = new ChannelManager(server, connection.Ip, connection.Port);
        newCM.RegisterChannel(new CubePositionStateChannel(new CubePositionProvider(), new TrivialStrategy(), 1000));
        _clients.Add(connection, newCM);
    }
}