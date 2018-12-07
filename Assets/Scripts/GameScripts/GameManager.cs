using System.Collections;
using System.Collections.Generic;
using EventNS.PlayerEventNS;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public GameObject Player;
	public bool isConnected;
	public PlayerEventClient eventClient;
	public bool isActive = false;

	private Health _health;
	private CanvasManager _canvasManager;
	
	private void Awake() {
		Instance = this;
		eventClient = GetComponent<PlayerEventClient>();
		_health = Player.GetComponent<Health>();
		_canvasManager = GetComponent<CanvasManager>();
	}

	private void Start() {
		Player.SetActive(false);
		isActive = false;
		eventClient.Connect();
	}

	public void Connected() {
		isActive = true;
	}

	public void Respawn() {
		Player.transform.position = Vector3.zero;
		PlayerEventClient.Instance.PlayerEventChannel.SendEvent(PlayerEvent.Respawn());
	}

	private void Update() {
		Player.SetActive(isActive);
		if(Input.GetKeyDown(KeyCode.R) && _health.GetCurrentHealth() <= 0) {
			Respawn();
		}
	}
}
