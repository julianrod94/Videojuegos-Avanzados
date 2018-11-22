using UnityEngine;

public class ServerGrenadeBehaviour: MonoBehaviour {
    private readonly float timeToExplode = 3;
    private float timeWhenThrown;

    private void Awake() {
        timeWhenThrown = Time.time;
    }

    private void Update() {
        if (Time.time - timeWhenThrown > timeToExplode) {
            ServerGameManager.Instance.ExplodeGrenade(transform.position);
            Destroy(gameObject);
        }
    }
}