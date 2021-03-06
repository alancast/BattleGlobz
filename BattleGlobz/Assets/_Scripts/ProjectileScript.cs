﻿using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public int 		ownerNum;
	public float 	throwAt = 0f;
	private float 	throwTimer = .1f;
	private float 	neutralThresh = 1000f;
	private int 	frameCounter = 0;
	//number of frames to wait before colliding with anything
	public int 		frameWait;
	private bool 	hasBounced = false;
	public bool 	neutralOnBounce;
	public Animator	ballAnim;
	//set when ball gets ejected, set in inspector
	public Material	whiteMat;

	void Awake(){
		ownerNum = -1;
		frameWait = 2;
		GetComponent<TrailRenderer>().material = whiteMat;
	}

	void OnCollisionStay(Collision collision){
		if (ownerNum == -1){
			OnCollisionEnter(collision);
		}
	}
	
	// Use this for initialization
	void OnCollisionEnter(Collision collision){
		if (frameCounter < frameWait){
			return;
		}
		Collider other = collision.collider;
		switch (other.gameObject.tag) {
		case "Player":
			PlayerControllerScript player = other.gameObject.GetComponent<PlayerControllerScript>();
			int otherPlayerNum = player.playerNum;
			// picked up by a player
			if(ownerNum == -1) {
				if(player.hasBall())
					return;
				Destroy(this.gameObject);
				other.gameObject.GetComponent<PlayerControllerScript>().pickUpProjectile();
			}
			else if(ownerNum != otherPlayerNum && !CameraScript.isBoss) {
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(ownerNum);
			}
			//Ball Collided with the owner
			else {
				if(Time.time > throwAt + throwTimer && !player.hasBall()){
					ownerNum = otherPlayerNum;
					Destroy(this.gameObject);
					other.gameObject.GetComponent<PlayerControllerScript>().pickUpProjectile();
				}
			}
			break;
		case "Shield":
			//This will make sure that you can't push a live ball across the floor
			if(ownerNum != -1){
				otherPlayerNum = other.transform.parent.GetComponent<PlayerControllerScript>().playerNum;

				GetComponent<Renderer> ().material = other.transform.parent.GetComponent<PlayerControllerScript> ().tempMat;
				GetComponent<TrailRenderer>().material = other.transform.parent.GetComponent<PlayerControllerScript> ().tempMat;
				ownerNum = otherPlayerNum;
				ballAnim.SetInteger("playerNum", ownerNum);
				other.transform.parent.GetComponent<PlayerControllerScript>().shieldAnimator.SetTrigger ("Hit");
			}
			break;
		case "Platform":

			Vector3 origin = this.transform.position;
			if (Physics.Raycast (origin, Vector3.down, GetComponent<Collider> ().bounds.size.y/2 + .7f)){
				hasBounced = true;
				if(neutralOnBounce){
					GetComponent<Renderer> ().material.color = Color.gray;
					GetComponent<TrailRenderer> ().material.color = Color.gray;
					ownerNum = -1;
				}
			}

			break;
		default:
			break;
		}
	}
	
	void Update () {		

		ballAnim.SetInteger("playerNum", ownerNum);

		Rigidbody tmp = this.GetComponent<Rigidbody> ();
		Vector3 tmpForce = -9f * tmp.mass * Vector3.up; 

		tmp.AddForce (tmpForce);

		if(frameCounter++ == frameWait)
			this.GetComponent<Collider> ().enabled = true;

		if (offScreen()) {
			Vector3 newPos = CameraScript.instance.transform.position;
			newPos.z = 0;
			this.transform.position = newPos;
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			
		}
		// if moving slowly and on the ground, make nuetral
		if(GetComponent<Rigidbody> ().velocity.sqrMagnitude < neutralThresh) {
			if(hasBounced){
				GetComponent<Renderer> ().material.color = Color.gray;
				GetComponent<TrailRenderer> ().material.color = Color.gray;
				ownerNum = -1;
			}
		}
	}
	
	//returns true if the ball is off the screen
	bool offScreen(){
		//amount off screen allowed before respawn
		float buffer = 1;
		//get game objects for left and right
		Transform left = CameraScript.instance.transform.GetChild(2);
		Transform right = CameraScript.instance.transform.GetChild(0);
		Transform top = CameraScript.instance.transform.GetChild(1);
		Transform bottom = CameraScript.instance.transform.GetChild(3);
		//check if off screen x
		if(transform.position.x < left.position.x - buffer || 
			transform.position.x > right.position.x + buffer){
			//print("ball off screen x");
			return true;  
		}
		//check if off screen y
		if(transform.position.y < bottom.position.y - (2*buffer) || 
		   transform.position.y > top.position.y + buffer){
			//print("ball off screen y");
			return true;  
		}
		return false;
	}
}