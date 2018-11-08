using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour {
	private GameObject player;
	private EnemyAnimation animation;
	public float hitRange = 3f;

	private bool hasAttacked = false;
	
	// Use this for initialization
	void Start () {
		player = PlayerCoordinator.Instance.player;
		animation = GetComponentInChildren<EnemyAnimation>();
	}

	// Update is called once per frame
	void Update () {
		if (isPlayerClose()) {
			if (!animation.attacking) {
				AudioManager.Instance.enemyHitting(GetComponentInParent<AudioSource>());
			}
			animation.attacking = true;
			
			if (animation.dealDamage) {
				if (!hasAttacked) {
					hasAttacked = true;
					PlayerCoordinator.Instance.dealDamage();
				}
			} else {
				hasAttacked = false;
			}
		} else {
			animation.attacking = false;
		}
	}

	public bool isPlayerClose() {
		return  Vector3.Distance(transform.position, player.transform.position) < hitRange;
	}
}
