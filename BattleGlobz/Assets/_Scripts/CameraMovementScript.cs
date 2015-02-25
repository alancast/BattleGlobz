using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour {
	static public CameraMovementScript instance;
	//amount the camera will jump on a kill
	float 			jumpSize = 2f;
	
	void Awake(){
		instance = this;
	}
	
	public void moveCamera(bool right){
		Vector3 nextPosition = transform.position;
		if (right){
			nextPosition.x += jumpSize;
			
		}
		else {
			nextPosition.x -= jumpSize;
		}
		transform.position = nextPosition;
	}
}
