using UnityEngine;
using System.Collections;

public class DeadHeadScript : MonoBehaviour {
	float lifetime = 3f;
	float killTime;
	
	void Awake(){
		killTime = Time.timeSinceLevelLoad + lifetime;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > killTime){
			Destroy(this.gameObject);
		}
	}
}
