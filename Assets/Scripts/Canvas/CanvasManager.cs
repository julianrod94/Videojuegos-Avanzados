using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

    public GameObject HeartOne;
    public GameObject HeartTwo;
    public GameObject HeartThree;
    public GameObject Player;
    public GameObject YouDied;
    public GameObject RespawnButton;
    private List<GameObject> hearts;
    public bool dead = false;


    private void Awake() {
        hearts = new List<GameObject>();
        hearts.Add(HeartOne);
        hearts.Add(HeartTwo);
        hearts.Add(HeartThree);
        DisableAll();
        HeartOne.SetActive(true);
        HeartTwo.SetActive(true);
        HeartThree.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
        var hp = Player.GetComponent<Health>().GetCurrentHealth();
        if (hp > 0) {
            DisableAll();
            for (int i = 0; i < hp; i++)
            {
                hearts[i].SetActive(true);
            }
           
        }
        else {
            ShowDied();
        }
    }

    public void ShowDied() {
        DisableAll();
        YouDied.SetActive(true);
        RespawnButton.SetActive(true);    
    }

    public void ShowHp() {
        DisableAll();
        HeartOne.SetActive(true);
        HeartTwo.SetActive(true);
        HeartThree.SetActive(true);
        dead = false;
    }

    private void DisableAll() {
        HeartOne.SetActive(false);
        HeartTwo.SetActive(false);
        HeartThree.SetActive(false);
        YouDied.SetActive(false);
        RespawnButton.SetActive(false);
    }
}