using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	//object will get destroyed after timeToLive expires
	float timeToLive;
	// Use this for initialization
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
