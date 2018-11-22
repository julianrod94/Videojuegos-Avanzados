using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public GameObject Player;
	public bool isConnected;
	public PlayerEventClient eventClient;
	public bool isActive = false;
	

	private void Awake() {
		Instance = this;
		eventClient = GetComponent<PlayerEventClient>();
	}

	private void Start() {
		Player.SetActive(false);
		isActive = false;
		eventClient.Connect();
	}

	public void Connected() {
		isActive = true;
	}

	private void Update() {
		Player.SetActive(isActive);
	}
}
