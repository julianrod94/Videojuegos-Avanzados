using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public GameObject Player;
	public bool isConnected;
	public PlayerEventClient eventClient;
	

	private void Awake() {
		Instance = this;
		GetComponent<PlayerEventClient>();
	}

	private void Start() {
		Player.SetActive(false);
		eventClient.Connect();
	}

	public void Connected() {
		Player.SetActive(true);
	}
}
