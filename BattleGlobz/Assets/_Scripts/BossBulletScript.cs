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
		else {
			Destroy(this.gameObject);
		}
	}
}
