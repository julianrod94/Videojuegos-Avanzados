using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnButton : MonoBehaviour {

	public void OnRespawn() {
		GameManager.Instance.Respawn();
	}
}
