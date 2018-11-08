using UnityEngine;

public class SelfDestruct : MonoBehaviour {

    public float livingTime = 1;

    void Start () {
        Invoke("SelfDesctruction",livingTime);
    }

    void SelfDesctruction() { Destroy(gameObject); }

}