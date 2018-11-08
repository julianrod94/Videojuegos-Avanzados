using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement: MonoBehaviour {
	// Update is called once per frame

	public float velocity = 5;
	//Running was to easy to escape from the enemies
	public float sprintFactor = 0f;
	public float backwardsPercentage = 0.5f;
	public float lateralPercentage = 0.8f;
	public float jumpForce = 5;
	
	private bool onFloor = false;
	private bool jumpCD = false;
	Rigidbody rbody;

	void Start () {
		// Obtenemos el RigidBody para hacer el salto posteriormente;
		rbody = GetComponent <Rigidbody> ();
		rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 

	}

	void FixedUpdate() {

		Vector3 movement = new Vector3();
		if (Input.GetKey(KeyCode.W)) {
			movement.z += velocity;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				movement.z *= (1 + sprintFactor);
			}
		}

		if (Input.GetKey(KeyCode.A)) {
			movement.x -= velocity * lateralPercentage;
		}

		if (Input.GetKey(KeyCode.D)) {
			movement.x += velocity * lateralPercentage;
		}

		if (Input.GetKey(KeyCode.S)) {
			movement.z -= velocity * backwardsPercentage;
		}

		if (onFloor && !jumpCD && Input.GetKeyDown(KeyCode.Space)) {
			jump();
		}
		
		gameObject.transform.Translate(movement * Time.deltaTime);
	}

	private void jump() {
		rbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
		jumpCD = true;
		Utils.Delay(100, () => { jumpCD = false; });
	}
	
	
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.layer == Layers.floor) {
			onFloor = true;
		}
	}


	void OnTriggerExit(Collider col){
		if (col.gameObject.layer == Layers.floor) {
			onFloor = false;
		}
	}
}

