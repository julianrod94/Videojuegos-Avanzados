using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

	public int CurrentHealth;

	private void Awake()
	{
		CurrentHealth = 3;
	}

	public void Damage(int currentHealth)
	{
		CurrentHealth = currentHealth;
	}

	public int GetCurrentHealth()
	{
		return CurrentHealth;
	}
}
