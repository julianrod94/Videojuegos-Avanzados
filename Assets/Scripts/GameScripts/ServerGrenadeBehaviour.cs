using UnityEngine;

public class ServerGrenadeBehaviour: MonoBehaviour {
    private readonly float timeToExplode = 5;
    private float timeWhenThrown;

    private void Awake() {
        timeWhenThrown = Time.time;
    }

    private void Update() {
        if (Time.time - timeWhenThrown > 5) {
            ServerGameManager.Instance.ExplodeGrenade(transform.position);
            
        }
    }
}