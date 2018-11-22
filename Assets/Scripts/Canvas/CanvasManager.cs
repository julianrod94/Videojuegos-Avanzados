using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

    public GameObject HeartOne;
    public GameObject HeartTwo;
    public GameObject HeartThree;
    public GameObject Player;
    public GameObject YouDied;
    public GameObject RespawnButton;
    private int _lastHP = 3;
    private List<GameObject> hearts;
    private bool dead = false;


    private void Awake() {
        hearts = new List<GameObject>();
        hearts.Add(HeartOne);
        hearts.Add(HeartTwo);
        hearts.Add(HeartThree);
    }

    // Update is called once per frame
    void Update ()
    {
        int currentHp = Player.GetComponent<Health>().GetCurrentHealth();
        if (_lastHP != currentHp)
        {
            DisableAll();
            for (int i = 0; i < currentHp; i++)
            {
                hearts[i].SetActive(true);
            }
            _lastHP = currentHp;
        }
    }

    public void ShowDied() {
        dead = true;
        DisableAll();
        YouDied.SetActive(true);
        RespawnButton.SetActive(true);    
    }

    public void ShowHp() {
        DisableAll();
        dead = false;
    }

    private void DisableAll() {
        HeartOne.SetActive(false);
        HeartTwo.SetActive(false);
        HeartThree.SetActive(false);
    }
}