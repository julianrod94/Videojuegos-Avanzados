using System.Collections.Generic;
using System.Linq;
using PackageProcessNS.KeepAliveNS;
using ServerNS;
using UnityEngine;

public class ServerGameManager: MonoBehaviour {
    
    public static ServerGameManager Instance;
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    
    private HashSet<int> toRespawn = new HashSet<int>();
    private HashSet<int> toDisconnect = new HashSet<int>();
    private HashSet<int> killed = new HashSet<int>();
    private readonly float explosionRadius = 25;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Update() {
        foreach (var playerId in toDisconnect) {
            if (toRespawn.Contains(playerId)) toRespawn.Remove(playerId);
            if (killed.Contains(playerId)) killed.Remove(playerId);
            if (players.ContainsKey(playerId)) Destroy(players[playerId]);
            players.Remove(playerId);
            ServerConnectionManager.Instance.RemovePlayer(playerId);
        }
        
        //Empty toRespawn
        if (toDisconnect.Count > 0) {
            toDisconnect = new HashSet<int>();
        }
        
        foreach (var playerId in toRespawn) {
            players[playerId].GetComponent<Health>().Respawn();
        }
        
        //Empty toRespawn
        if (toRespawn.Count > 0) {
            toRespawn = new HashSet<int>();
        }

        var toRemove = players.Where((p) => p.Value.GetComponent<Health>().CurrentHealth <= 0).ToList();
        
        foreach (var playerId in toRemove) {
            KillPlayer(playerId.Key);
        }
    }

    public void AddPlayer(GameObject player, int id, ChannelManager cm) {
        players.Add(id, player);
        ServerKeepAliveManager.Instance.AddPlayer(id, cm);
    }

    public void ExplodeGrenade(Vector3 position) {
        foreach (var player in players.Keys) {
            if (Vector3.Distance(players[player].transform.position, position) < explosionRadius) {
                players[player].GetComponent<Health>().Damage();
            }
        }
    }

    public bool IsPlayerDead(int id) {
        return killed.Contains(id);
    }

    private void KillPlayer(int id) {
        killed.Add(id);
        Destroy(players[id].gameObject);
        players.Remove(id);
        OtherPlayersStatesProvider.Instance.Healths.Remove(id);
    }

    public void RespawnPlayer(int id) {
        if (IsPlayerDead(id)) { 
            killed.Remove(id);    
            toRespawn.Add(id);
        }
    }
    
    public void DisconnectPlayer(int id) {
        toDisconnect.Add(id);
    }
}