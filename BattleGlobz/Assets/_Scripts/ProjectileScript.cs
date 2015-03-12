using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public int ownerNum;
	public float throwAt = 0f;
	private float throwTimer = .1f;

	void OnTriggerStay(Collider other){
		if (ownerNum == -1){
			OnTriggerEnter(other);
		}
	}

	// Use this for initialization
	void OnTriggerEnter(Collider other){
		switch (other.gameObject.tag) {
			case "Player":
				int otherPlayerNum = other.gameObject.GetComponent<PlayerControllerScript>().playerNum;
				// picked up by a player
				if(ownerNum == -1) {
					print("picked up");
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
				print ("Shield collision");
				otherPlayerNum = other.transform.parent.parent.GetComponent<PlayerControllerScript>().playerNum;
				GetComponent<Renderer> ().material = other.transform.parent.parent.GetComponent<Renderer> ().material;
				ownerNum = otherPlayerNum;
				break;
			case "Platform":
				Vector3 origin = this.transform.position;
				if (Physics.Raycast (origin, Vector3.down, GetComponent<Collider> ().bounds.size.y/2 + .05f)){
					GetComponent<Renderer> ().material.color = Color.gray;
					ownerNum = -1;
				}
				break;
			default:
				break;
		}
	}
	
	void Update () {
		// if moving slowly and on the ground, make nuetral
		if(GetComponent<Rigidbody> ().velocity.sqrMagnitude < 2f) {
			if (Physics.Raycast (transform.position, Vector3.down, GetComponent<Collider> ().bounds.size.y + .05f)) {
				GetComponent<Renderer> ().material.color = Color.gray;
				ownerNum = -1;
			}
		}
	}
}
