using UnityEngine;
using System.Collections;

public class FollowCamScript : MonoBehaviour {
	//instance of followcam so that it can be
/*	//managed from anywhere (likely needed later)
	public static FollowCamScript 	instance;
	//point of interest of what the camera is following
	public GameObject 	poi;
	Camera thisCamera;
	float minX = Mathf.Infinity;
	float maxX = Mathf.NegativeInfinity;
	
	void Awake() {
		instance = this;
		if (!poi) print ("missing a poi for FollowCamScript");

		thisCamera = GetComponent<Camera> ();
		this.transform.position = new Vector3 (1f, 0, 0);
		thisCamera.orthographicSize = 22f;

	}
	
	// Update is called once per frame
	void Update () {
		//PlayerControllerScript players[] = GameObject.GetComponents<PlayerControllerScript> ();
		PlayerControllerScript[] players =  FindObjectsOfType<PlayerControllerScript> ();


		foreach(PlayerControllerScript player in players){
			minX = Mathf.Min (minX, player.transform.position.x);
			maxX = Mathf.Max (maxX, player.transform.position.x);
		}

		float distance = maxX - minX;

		Vector3 nextPosition = new Vector3.zero();
		nextPosition.z = -10;
		nextPosition.x = minX + distance/2;
		transform.position = nextPosition;
		thisCamera.orthographicSize = distance ;
	}*/
}
