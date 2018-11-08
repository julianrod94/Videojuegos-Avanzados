using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour {

	private Animator animator;
	public bool dealDamage = false;
	private GameObject player;

	private bool _attacking = false;
	public bool attacking {
		get { return _attacking; }
		set {
			if (value != _attacking) {
				animator.SetBool(AnimatorParameters.enemyAttacking, value);
			}
			_attacking = value;
		}
	}
	
	private bool _dead = false;
	public bool dead {
		get { return _dead; }
		set {
			if (value != _dead) {
				animator.SetBool(AnimatorParameters.enemyDead, value);
			}
			_dead = value;
		}
	}

	void Start() {
		animator = GetComponentInChildren<Animator>();
		player = PlayerCoordinator.Instance.player;
	}

	// Update is called once per frame
	void Update () {
		transform.LookAt(player.transform.position);
		transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y,0);
		if (shouldDealDamage()) {
			dealDamage = true;
		} else {
			dealDamage = false;
		}
	}

	private bool shouldDealDamage() {
		if (animator == null) {
			return false;
		}
		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
		return state.IsName(AnimatorStates.dealDamage);

	}
	
}
