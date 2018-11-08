using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder : MonoBehaviour {
	public float reachLimit = 5f;
	private PlayerCoordinator pc;

	void Start() {
		pc = PlayerCoordinator.Instance;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.E)) {
			if (pc.holding) {
				pc.dropBall();
			} else if(Vector3.Distance(transform.position, pc.ball.transform.position) < reachLimit) {
				pc.grabBall();
			}
		}
	}
}
