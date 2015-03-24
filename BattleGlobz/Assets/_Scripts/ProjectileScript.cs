using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public int ownerNum;
	public float throwAt = 0f;
	private float throwTimer = .1f;
	private float neutralThresh = 1000f;
	private int frameCounter = 0;


	void OnCollisionStay(Collision collision){
		if (ownerNum == -1){
			OnCollisionEnter(collision);
		}
	}
	
	// Use this for initialization
	void OnCollisionEnter(Collision collision){
		Collider other = collision.collider;
		print ("collision");
		switch (other.gameObject.tag) {
		case "Player":
			PlayerControllerScript player = other.gameObject.GetComponent<PlayerControllerScript>();
			int otherPlayerNum = player.playerNum;
			// picked up by a player
			if(ownerNum == -1) {
				ownerNum = otherPlayerNum;
				Destroy(this.gameObject);
				other.gameObject.GetComponent<PlayerControllerScript>().pickUpProjectile();
			}
			else if(ownerNum != otherPlayerNum) {
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(ownerNum);
			}
			//Ball Collided with the owner
			else {
				if(Time.time > throwAt + throwTimer){
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
				GetComponent<Renderer> ().material = other.transform.parent.GetComponent<Renderer> ().material;
				ownerNum = otherPlayerNum;
			}
			break;
		case "Platform":

			Vector3 origin = this.transform.position;
			if (Physics.Raycast (origin, Vector3.down, GetComponent<Collider> ().bounds.size.y/2 + .7f)){
				GetComponent<Renderer> ().material.color = Color.gray;
				ownerNum = -1;
			}

			break;
		default:
			break;
		}
	}
	
	void Update () {
		if(frameCounter++ == 1)
			this.GetComponent<Collider> ().enabled = true;

		if (this.transform.position.y <= -50) {
			Vector3 newPos = Camera.current.transform.position;
			newPos.z = 0;
			this.transform.position = newPos;
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			
		}
		// if moving slowly and on the ground, make nuetral
		if(GetComponent<Rigidbody> ().velocity.sqrMagnitude < neutralThresh) {
			if (Physics.Raycast (transform.position, Vector3.down, GetComponent<Collider> ().bounds.size.y/2 + .2f)) {
				GetComponent<Renderer> ().material.color = Color.gray;
				ownerNum = -1;
			}
		}
	}
}