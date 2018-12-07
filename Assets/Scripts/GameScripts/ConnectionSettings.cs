using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionSettings : MonoBehaviour {
	public static ConnectionSettings Instance;
	
	[Range(0, 1000)]
	public int Latency;
	
	[Range(0, 100)]
	public float PacketLoss;

	public bool Predict;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}
	}
}
