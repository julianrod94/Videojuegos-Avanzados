using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
	// Update is called once per frame
	public float weaponRange = 50f;
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			PlayerCoordinator.Instance.shoot(weaponRange,transform);
		}
	}
}
