using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour {
	private NavMeshAgent agent;
	private float lastVel = 0;
	private bool alive = true;
	private EnemyAnimation enemyAnimation;
	private bool hasTarget = false;

	private bool stopped = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		enemyAnimation = GetComponentInChildren<EnemyAnimation>();
	}
	
	// Update is called once per frame
	void Update() {
		if (!alive) {
			return;
		}
		
		if (enemyAnimation.attacking || stopped) {
			Stop();
			return;
		}
		
		Vector3 dest = PlayerCoordinator.Instance.player.transform.position;
		float currentVel = agent.velocity.magnitude;

		if (lastVel - 0.1 > currentVel) {
			//Stop enemy from sliding, if it is desaccelerating, set its velocitry to 0
			Stop();
			lastVel = 0;
		} else {
			lastVel = currentVel;
		}

		if (!hasTarget) {
			agent.SetDestination(dest);
			CDUtils.Delay(200,() => { hasTarget = false; });
		}
		transform.LookAt(dest);
	}
	
	
	void Stop() {
		agent.velocity = Vector3.zero;
	}

	public void Die() {
		SpawnerCoordinator.Instance.enemies--;
		alive = false;
		if (agent.isActiveAndEnabled) {
			agent.isStopped = true;
		}
		Collider[] colliders = GetComponentsInChildren<Collider>();
		for (int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = false;
		}
		enemyAnimation.dead = true;
		agent.enabled = false;
		
		Invoke("Destroy",2);
	}
	
	public void Hit() {
		Stop();
		StartCoroutine(wait(0.2f));
	}


	void Destroy() {
		Destroy(gameObject);
	}

	IEnumerator wait(float i) {
		stopped = true;
		yield return new WaitForSeconds(i);
		stopped = false;
	}
}
