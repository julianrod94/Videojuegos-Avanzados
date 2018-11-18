using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
	// Update is called once per frame
	public float weaponRange = 50f;
	private PlayerEventClient client;

	void Start() {
		client = GetComponentInParent<PlayerEventClient>();
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			client.Shoot(weaponRange,transform);
		}
	}
}
