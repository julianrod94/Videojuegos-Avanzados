using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

	public int CurrentHealth;

	private void Awake() {
		CurrentHealth = 3;
	}

	
	public void Damage() {
		CurrentHealth--;
		CurrentHealth = CurrentHealth >= 0 ? CurrentHealth : 0;
	}
	public void setHealth(int currentHealth) {
		CurrentHealth = currentHealth >= 0 ? currentHealth : 0;
	}

	public int GetCurrentHealth() {
		return CurrentHealth;
	}

	public void Respawn() {
		transform.position = Vector3.zero;
		CurrentHealth = 3;
	}
}
