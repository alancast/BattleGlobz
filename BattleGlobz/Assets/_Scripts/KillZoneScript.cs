using UnityEngine;
using System.Collections;

public class KillZoneScript : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			PlayerControllerScript player = other.gameObject.GetComponent<PlayerControllerScript>();
			int otherPlayerNum = player.playerNum;
			other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(-4);
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, transform.localScale);
	}
}
