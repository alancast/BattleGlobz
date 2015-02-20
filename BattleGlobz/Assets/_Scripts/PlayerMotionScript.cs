using UnityEngine;
using System.Collections;

public class PlayerMotionScript : MonoBehaviour {
	//instance of the Player that can be accessed from anywhere
	public static PlayerMotionScript	instance;
	//Rigidbody so that only need to call getComponent once
	public Rigidbody	thisRigidbody;
	//accelerations for horizontal movement and jumping
	float				xAccel = 20;
	float				jumpAccel = 20;
	//maximum speeds (directionless) for horizontal and jumping
	float 				maxXSpeed = 20;
	float 				maxJumpSpeed = 10;
	
	void Awake(){
		instance = this;
		thisRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftArrow)){
			if (thisRigidbody.velocity.x > -maxXSpeed){
				thisRigidbody.AddForce(Vector3.left*xAccel);
			}
		}
		if (Input.GetKey(KeyCode.RightArrow)){
			if (thisRigidbody.velocity.x < maxXSpeed){
				thisRigidbody.AddForce(Vector3.right*xAccel);
			}
		}
		if (Input.GetKey(KeyCode.UpArrow)){
			if (thisRigidbody.velocity.y < maxJumpSpeed){
				thisRigidbody.AddForce(Vector3.up*jumpAccel);
			}
		}
	}
}
