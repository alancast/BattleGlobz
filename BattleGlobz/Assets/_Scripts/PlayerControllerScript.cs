using UnityEngine;
using System.Collections;

public class PlayerControllerScript : MonoBehaviour {
	//instance of the Player that can be accessed from anywhere
	public static PlayerControllerScript	instance;
	//Rigidbody so that only need to call getComponent once
	public Rigidbody	thisRigidbody;
	//must be added in inspector
	//what will be shot when you fire the gun
	public GameObject	projectile;
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
		if (Input.GetKeyDown(KeyCode.Space)){
			shootProjectile(thisRigidbody.velocity);
		}
	}
	
	//will instantiate a projectile with initial velocity "velocity" passed in
	void shootProjectile(Vector3 velocity){
		GameObject temp = (GameObject)Instantiate(projectile, transform.position, 
													Quaternion.Euler(Vector3.zero));
		temp.rigidbody.velocity = velocity;
	}
}
