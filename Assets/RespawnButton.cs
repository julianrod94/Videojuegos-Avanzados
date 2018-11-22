using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnButton : MonoBehaviour {
	private void Update() {
		if(Input.GetKeyDown(KeyCode.R)) {
			Debug.Log("adsadas");
			GameManager.Instance.Respawn();
		}
	}

	public void OnRespawn() {
		
	}
}
