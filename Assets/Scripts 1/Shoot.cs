using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
	// Update is called once per frame
	public float weaponRange = 50f;
	private PlayerEventClient client;
	private Health _health;

	void Start() {
		client = GameManager.Instance.eventClient;
		_health = GetComponentInParent<Health>();
	}
	
	void Update () {

		if(_health.GetCurrentHealth() <=0 ) { return; }
		if (Input.GetMouseButtonDown(0)) {
			client.Shoot(weaponRange,transform);
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			client.ThrowGranade(Camera.main.transform.rotation.eulerAngles);
		}
	}
}
