using UnityEngine;
using System.Collections;

public class BossBulletScript : MonoBehaviour {
	public int ownerNum;
	public float throwAt = 0f;
	private float throwTimer = .1f;
	
	// Use this for initialization
	void OnTriggerEnter(Collider other){
		switch (other.gameObject.tag) {
		case "Player":
			PlayerControllerScript player = other.gameObject.GetComponent<PlayerControllerScript>();
			int otherPlayerNum = player.playerNum;
			//bullet collided with another player
			if(ownerNum != otherPlayerNum) {
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(ownerNum);
				Destroy(this.gameObject);
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
			Destroy(this.gameObject);
			break;
		default:
			break;
		}
	}
}
