using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionSettings : MonoBehaviour {
	public static ConnectionSettings Instance;


	public bool Predict;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}
	}
}
