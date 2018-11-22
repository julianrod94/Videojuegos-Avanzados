using UnityEngine;

public class ServerGrenadeBehaviour: MonoBehaviour {
    private readonly float timeToExplode = 3;
    private float timeWhenThrown;
    public bool isExploding;

    private void Awake() {
        timeWhenThrown = Time.time;
    }

    private void Update() {
        if (!isExploding && Time.time - timeWhenThrown > timeToExplode) {
            ServerGameManager.Instance.ExplodeGrenade(transform.position);
            isExploding = true;
        }

        if (Time.time - timeWhenThrown > timeToExplode + 1) {
            Destroy(gameObject);
        }
    }
}