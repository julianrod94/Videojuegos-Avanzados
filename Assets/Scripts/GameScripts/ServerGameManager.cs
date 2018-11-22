using System.Collections.Generic;
using PackageProcessNS.KeepAliveNS;
using ServerNS;
using UnityEngine;

public class ServerGameManager: MonoBehaviour {
    
    
    public static ServerGameManager Instance;
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private List<int> toRespawn = new List<int>();
    private List<int> toRemove = new List<int>();
    private readonly float explosionRadius = 25;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Update() {
        foreach (var playerId in toRespawn) {
            players[playerId].GetComponent<Health>().CurrentHealth = 3;
            players[playerId].transform.position = Vector3.zero;
        }

        if (toRespawn.Count > 0) {
            toRespawn = new List<int>();
        }

        foreach (var playerId in toRemove) {
            Destroy(players[playerId].gameObject);
            players.Remove(playerId);
        }

        if (toRemove.Count > 0) {
            toRemove = new List<int>();
        }
    }

    public void AddPlayer(GameObject player, int id, ChannelManager cm) {
        players.Add(id, player);
//        ServerKeepAliveManager.Instance.AddPlayer(id, cm);
    }

    public void ExplodeGrenade(Vector3 position) {
        foreach (var player in players.Keys) {
            if (Vector3.Distance(players[player].transform.position, position) < explosionRadius) {
                var health = players[player].GetComponent<Health>();
                health.Damage(health.CurrentHealth - 1);
            }
        }
    }

    public void RespawnPlayer(int id) {
        toRespawn.Add(id);
    }
    
    public void RemovePlayer(int id) {
        toRemove.Add(id);
    }
}