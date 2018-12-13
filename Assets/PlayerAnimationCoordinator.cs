using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationCoordinator : MonoBehaviour {
	private Animator _animator;
	private Rigidbody _rigidbody;

	private float deadZone = 0.2f;
	
	// Use this for initialization
	void Start () {
		_animator = GetComponentInChildren<Animator>();
		_rigidbody = GetComponentInChildren<Rigidbody>();
	}

	private Vector3 _lastPosition = Vector3.zero;
	// Update is called once per frame
	void Update () {
		var velocity = transform.InverseTransformDirection(_lastPosition - transform.position);
		_lastPosition = transform.position;
		Debug.Log(velocity);

		_animator.SetBool("forward", velocity.z < -deadZone);
		_animator.SetBool("right", velocity.x < -deadZone);
		_animator.SetBool("back", velocity.z > deadZone);
		_animator.SetBool("left", velocity.x > deadZone);

	}
}
