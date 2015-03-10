using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public int ownerNum;

	// Use this for initialization
	void OnTriggerEnter(Collider other){
		switch (other.gameObject.tag) {
<<<<<<< HEAD
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
					other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(false);
				}
				else { // ownerNum == otherPlayerNum
					//
				}
				break;
			default:
=======
		case "Player0":
			if(this.tag == "Bullet0" )
				return;
			else
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(true, 1);
				Destroy(this.gameObject);
				break;
		case "Player1":
			if(this.tag == "Bullet1" )
				return;
			else
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(false, 0);
				Destroy(this.gameObject);
>>>>>>> Alex
				break;
<<<<<<< HEAD
		default:
			if(other.tag == "Bullet1" || other.tag == "Bullet0")
				return;
			else
				Destroy(this.gameObject);
			break;
=======
>>>>>>> JakeScatter
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
