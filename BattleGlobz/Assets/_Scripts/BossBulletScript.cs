using UnityEngine;
using System.Collections;

public class BossBulletScript : MonoBehaviour {
	public int ownerNum;
	public float throwAt = 0f;
	private float throwTimer = .1f;

	public GameObject squishedBullet;
	
	// Use this for initialization
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Boss" || other.gameObject.tag == "BossBullet"){

		}
		else if (other.gameObject.tag == "Player"){
			PlayerControllerScript player = other.gameObject.GetComponent<PlayerControllerScript>();
			int otherPlayerNum = player.playerNum;
			if (other.gameObject.GetComponent<PlayerControllerScript> ().isFrozen){
				return;
			}
			other.gameObject.GetComponent<PlayerControllerScript> ().isFrozen = true;
			CameraScript.playerCountAlive--;
			Destroy(this.gameObject);

			Vector3 squishPos = transform.position;
			squishPos.y -= 1.4f;
			Instantiate(squishedBullet, squishPos, Quaternion.Euler(Vector3.zero));
		}
		else {
			Instantiate(squishedBullet, transform.position, Quaternion.Euler(Vector3.zero));
			Destroy(this.gameObject);
		}
	}
}
