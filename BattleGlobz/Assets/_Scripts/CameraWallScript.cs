using UnityEngine;
using System.Collections;

public class CameraWallScript : MonoBehaviour {
	//direction camera is moving
	static public bool		right = false;
	//if the camera is moving or not
	static public bool		moving = false;
	//time in which you make moving false
	static public float 	stopMoving;
	//is left or right wall
	public bool		wallIsLeft = false;
	
	void Awake(){
		stopMoving = Time.time;
	}
	
	void Update(){
		if (moving && Time.time > stopMoving) moving = false;
	}
	
	void OnTriggerEnter(Collider other){
		if (!moving) return;
		Vector3 nextPosition = other.transform.position;
		if (other.tag == "Player0" || other.tag == "Player1"){
			if (right){
				nextPosition.x += 1;
			}
			else{
				nextPosition.x -= 1;
			}
		}
		//don't push this character left or it puts him off map
		if (other.tag == "Player0" && !right && wallIsLeft){
			nextPosition.x += 1;
		}
		//don't push this character right or it puts him off map
		if (other.tag == "Player1" && right && !wallIsLeft){
			nextPosition.x -= 1;
		}
		other.transform.position = nextPosition;
	}
	
	void OnTriggerStay(Collider other){
		OnTriggerEnter(other);
	}
}
