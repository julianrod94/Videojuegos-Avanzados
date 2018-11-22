using System;
using System.Collections.Generic;
using System.Net;
using ChannelNS;
using EventNS.PlayerEventNS;
using SenderStrategyNS;
using ServerNS;
using UnityEngine;

public class ServerConnectionManager : MonoBehaviour {
    public struct Connection {
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

    public GameObject playerPrefab;
    public static ServerConnectionManager Instance;
    
    public int Port;
    private Server server;
    private readonly Dictionary<Connection, ChannelManager> _clients = new Dictionary<Connection, ChannelManager>();
    private readonly Dictionary<int, ChannelManager> _conections = new Dictionary<int, ChannelManager>();
    public int initializedPlayers = 0;
    
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

    public void sendToChannel(IPAddress ip, int port, byte[] packet) {
        Connection connection = new Connection(ip, port);
        if (!_clients.ContainsKey(connection)) addNewClient(connection);
        _clients[connection].ReceivePacket(packet);
    }

    private void addNewClient(Connection connection) {
        ChannelManager newCM = new ChannelManager(server, connection.Ip, connection.Port);
        _conections.Add(_clients.Count, newCM);
        _clients.Add(connection, newCM);
    }

    private void FixedUpdate() {
        while (initializedPlayers < _clients.Count) {
            var cm = _conections[initializedPlayers];
            var newPlayer = Instantiate(playerPrefab);
            var health = newPlayer.GetComponent<Health>();
            var id = OtherPlayersStatesProvider.Instance.AddPlayer(health, cm);
            newPlayer.GetComponent<OtherPlayer>().id = id;
            newPlayer.GetComponent<PlayerMovementProvider>().SetupChannels(cm);
            GrenadeStatesProvider.Instance.SetupChannel(id, cm);
            ServerGameManager.Instance.AddPlayer(newPlayer, id, cm);
            var pEvent = newPlayer.GetComponent<PlayerEventServer>();
            pEvent.SetupChannels(cm);
            initializedPlayers++;
            pEvent.PlayerEventChannel.SendEvent(PlayerEvent.Connected());
        }
    }

    private void OnDestroy() {
        server.Destroy();
    }
}