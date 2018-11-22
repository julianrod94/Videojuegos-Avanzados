using System.Collections.Generic;
using UnityEngine;

public class ServerGameManager: MonoBehaviour {
    
    
    public static ServerGameManager Instance;
    private List<GameObject> players = new List<GameObject>();
    private readonly float explosionRadius = 5;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void AddPlayer(GameObject player) {
        players.Add(player);
    }

    public void ExplodeGrenade(Vector3 position) {
        foreach (var player in players) {
            if (Vector3.Distance(player.transform.position, position) < explosionRadius) {
                var health = player.GetComponent<Health>();
                health.Damage(health.CurrentHealth - 1);
            }
        }
    }
    
    public void RemovePlayer(int id) {
        foreach (var player in players) {
            if (player.GetComponent<OtherPlayer>().id == id) {
                Destroy(player.gameObject);
            }
        }
    }
}