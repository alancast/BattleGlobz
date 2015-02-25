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
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(true);
				Destroy(this.gameObject);
			break;
		case "Player1":
			if(this.tag == "Bullet1" )
				return;
			else
				other.gameObject.GetComponent<PlayerControllerScript> ().handleDeath(false);
				Destroy(this.gameObject);
				break;
		default:
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
