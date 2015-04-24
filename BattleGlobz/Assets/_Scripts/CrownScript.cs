using UnityEngine;
using System.Collections;

public class CrownScript : MonoBehaviour {

	void OnCollisionEnter(Collision collision){
		Collider other = collision.collider;
		if (other.gameObject.tag == "Player") {
			int num = other.gameObject.GetComponent<PlayerControllerScript> ().playerNum;
			FindObjectOfType<BossScript> ().triggerCutScene (num);
			CameraScript.isBoss = true;
			Material mat = null;
			mat = other.gameObject.GetComponent<PlayerControllerScript>().tempMat;
			FindObjectOfType<BossScript> ().CreateBoss (num, transform.position, mat);
			Destroy(other.gameObject);
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
