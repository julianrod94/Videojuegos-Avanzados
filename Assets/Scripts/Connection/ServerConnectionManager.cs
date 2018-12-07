using System;
using System.Collections.Generic;
using System.Net;
using ChannelNS;
using EventNS.PlayerEventNS;
using SenderStrategyNS;
using ServerNS;
using UnityEngine;
using Utils;

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
    
    private readonly Dictionary<int, Connection> _connectionsInfo = new Dictionary<int, Connection>();
    private readonly Dictionary<Connection, ChannelManager> _clients = new Dictionary<Connection, ChannelManager>();
    private readonly Dictionary<int, ChannelManager> _conections = new Dictionary<int, ChannelManager>();
    public int connectedNonce = 0;
    public int initializedNonce = 0;
    
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
        _conections.Add(connectedNonce, newCM);
        _connectionsInfo.Add(connectedNonce, connection);
        _clients.Add(connection, newCM);
        connectedNonce++;
    }

    private void FixedUpdate() {
        CurrentTime.Time = Time.time;
        while (initializedNonce < connectedNonce) {
            var id = initializedNonce;
            
            var cm = _conections[id];
            var newPlayer = Instantiate(playerPrefab);
            var health = newPlayer.GetComponent<Health>();
            
            OtherPlayersStatesProvider.Instance.AddPlayer(id, health, cm);
            GrenadeStatesProvider.Instance.SetupChannel(id, cm);
            ServerGameManager.Instance.AddPlayer(newPlayer, id, cm);

            newPlayer.GetComponent<PlayerMovementProvider>().SetupChannels(cm);
            newPlayer.GetComponent<OtherPlayer>().id = id;
            newPlayer.GetComponent<PlayerEventServer>().SetupChannels(cm);
            
            initializedNonce++;
        }
    }

    public void RemovePlayer(int id) {
        OtherPlayersStatesProvider.Instance.DisconnectPlayer(id);
        GrenadeStatesProvider.Instance.DisconnectPlayer(id);
        
        _conections.Remove(id);
        var connection = _connectionsInfo[id];
        _connectionsInfo.Remove(id);
        _clients.Remove(connection);
    }

    private void OnDestroy() {
        server.Destroy();
    }
}