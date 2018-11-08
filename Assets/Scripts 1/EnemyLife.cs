using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour {

	public float life = 100f;
	private FollowPlayer followPlayer;

	// Use this for initialization
	void Start () {
		followPlayer = GetComponentInParent<FollowPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void damage(float amount) {
		print(amount);
		life -= amount;
		if (life < 0) {
			followPlayer.Die();
		} else {
			followPlayer.Hit();
		}


	}
}
