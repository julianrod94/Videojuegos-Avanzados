using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour {
	private GameObject player;
	private EnemyAnimation enemyAnimation;
	public float hitRange = 3f;

	private bool hasAttacked = false;
	
	// Use this for initialization
	void Start () {
		player = PlayerCoordinator.Instance.player;
		enemyAnimation = GetComponentInChildren<EnemyAnimation>();
	}

	// Update is called once per frame
	void Update () {
		if (isPlayerClose()) {
			if (!enemyAnimation.attacking) {
				AudioManager.Instance.enemyHitting(GetComponentInParent<AudioSource>());
			}
			enemyAnimation.attacking = true;
			
			if (enemyAnimation.dealDamage) {
				if (!hasAttacked) {
					hasAttacked = true;
					PlayerCoordinator.Instance.dealDamage();
				}
			} else {
				hasAttacked = false;
			}
		} else {
			enemyAnimation.attacking = false;
		}
	}

	public bool isPlayerClose() {
		return  Vector3.Distance(transform.position, player.transform.position) < hitRange;
	}
}
