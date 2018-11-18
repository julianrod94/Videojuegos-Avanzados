using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
	// Update is called once per frame
	public float weaponRange = 50f;
	public PlayerEventClient client;
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			client.Shoot(weaponRange,transform);
		}
	}
}
