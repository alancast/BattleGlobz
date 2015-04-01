using UnityEngine;
using System.Collections;

public class BossBulletScript : MonoBehaviour {
	public int ownerNum;
	public float throwAt = 0f;
	private float throwTimer = .1f;
	
	// Use this for initialization
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Boss" || other.gameObject.tag == "BossBullet"){

		}
		else if (other.gameObject.tag == "Player"){
			PlayerControllerScript player = other.gameObject.GetComponent<PlayerControllerScript>();
			int otherPlayerNum = player.playerNum;
			other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(ownerNum);
			Destroy(this.gameObject);
		}
		else {
			Destroy(this.gameObject);
		}
	}
}
