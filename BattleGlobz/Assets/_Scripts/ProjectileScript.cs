using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	//object will get destroyed after timeToLive expires
	float timeToLive;
	// Use this for initialization
	void OnTriggerEnter(Collider other){
		switch (other.gameObject.tag) {
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
				break;
		default:
			if(this.tag == "Bullet1" || this.tag == "Bullet0")
				return;
			else
				Destroy(this.gameObject);
			break;
		}
	}

	void Start () {
		timeToLive = Time.time + 3f;
	}
	
	// Update is called once per frame
	void Update () {
		if (timeToLive < Time.time){
			Destroy(this.gameObject);
		}
	}
}
