using UnityEngine;
using System.Collections;

public class LevelMovementScript : MonoBehaviour {
	public static bool	stopMoving = false;
	float 				moveAmount = .01f;

	void FixedUpdate(){
		if (stopMoving){
			return;
		}
		Vector3 nextPosition = transform.position;
		nextPosition.x -= moveAmount;			
		transform.position = nextPosition;
	}
}
