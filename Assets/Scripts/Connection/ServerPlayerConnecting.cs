using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ServerPlayerConnecting : MonoBehaviour {

	public GameObject playerPrefab;

	public static ServerPlayerConnecting Instance;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}
	}
	
	// Use this for initialization
	void Start () {
		ServerConnectionManager.Instance.AddInitializer(Initialize);
	}

	void Initialize(ChannelManager cm) {
		var newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		newPlayer.GetComponent<PlayerMovementProvider>().SetupChannels(cm);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
