using UnityEngine;
using System.Collections;

public class FollowCamScript : MonoBehaviour {
	//instance of followcam so that it can be
	//managed from anywhere (likely needed later)
	public static FollowCamScript 	instance;
	//point of interest of what the camera is following
	public GameObject 	poi;
	
	void Awake() {
		instance = this;
		if (!poi) print ("missing a poi for FollowCamScript");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 nextPosition = poi.transform.position;
		nextPosition.z = -10;
		transform.position = nextPosition;	
	}
}
