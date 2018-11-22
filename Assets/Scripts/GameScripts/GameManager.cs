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

	public void Die() {
		isActive = false;
		//TODO DISABLE ACTIONS
		GetComponent<CanvasManager>().ShowDied();

	}

	public void Respawn() {
		isActive = true;
		Player.GetComponent<Health>().CurrentHealth = 3;
		Player.transform.position = Vector3.zero;
		GetComponent<CanvasManager>().ShowHp();
	}

	private void Update() {
		Player.SetActive(isActive);
	}
}
