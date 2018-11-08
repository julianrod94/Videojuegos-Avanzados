using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour {

	// Use this for initialization
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag(Tags.ball)) {
			ScenesOrganizer.Instance.goToVictory();
		}	
	}
}
