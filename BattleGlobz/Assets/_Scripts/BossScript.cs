using UnityEngine;
using System.Collections;
using InControl;

public class BossScript : MonoBehaviour {

	//Rigidbody so that only need to call getComponent once
	public Rigidbody	thisRigidbody;
	int					playerNum = -1;
	int					health = 10;
	float				speed = 5;
	public GameObject	laserPrefab;
	GameObject			laser;
	GameObject			gun;
	bool				laserUp = false;
	bool				onCoolDown;
	float				coolDownLength = 1f;
	float				coolDownTime = 0f;

	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		gun = transform.GetChild(0).gameObject;
		laser = transform.GetChild (1).gameObject;

	
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Projectile") {
			print (health);
			health--;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0) {
			print ("I'm dead");
			Destroy (this.gameObject);
		}
		if (playerNum == -1)
			return;
		if(!laserUp && !onCoolDown)
			handleMovement ();
		handleLaser ();
		if (Time.time > coolDownTime + coolDownLength){
			print ("Resest");
			onCoolDown = false;
		}
	}

	void handleLaser(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (gameController.RightTrigger.WasPressed && !onCoolDown) {
			thisRigidbody.velocity = Vector3.zero;
			laser.GetComponent<Renderer>().enabled = true;
			laser.GetComponent<Collider>().enabled = true;
			laserUp = true;
		}
		if (gameController.RightTrigger.WasReleased && !onCoolDown) {
			coolDownTime = Time.time;
			onCoolDown = true;
			laserUp = false;
			laser.GetComponent<Renderer>().enabled = false;
			laser.GetComponent<Collider>().enabled = false;
		}

		if (rightStickPressed() && !laserUp){
			float rot = rotationAmount();
			gun.transform.RotateAround(transform.position, Vector3.forward, rot);
			laser.transform.RotateAround(transform.position, Vector3.forward, rot);
		}

	}

	bool rightStickPressed(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		float threshold = .2f;
		if (Mathf.Abs(gameController.RightStickX) > threshold ||
		    Mathf.Abs(gameController.RightStickY) > threshold){
			return true;
		}
		return false;
	}

	//returns the amount needed to rotate from where gun
	//currently is, to where it should be with the RightStick
	float rotationAmount(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		float y = gameController.RightStickY;
		float x = gameController.RightStickX;
		float oldAngle = gun.transform.eulerAngles.z;
		float newAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
		return newAngle - oldAngle;
	}

	void handleMovement(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (gameController.Direction.Left.IsPressed) {
			Vector3 vel = thisRigidbody.velocity;
			vel.x = -speed;
			thisRigidbody.velocity = vel;
		}
		if (gameController.Direction.Right.IsPressed) {
			Vector3 vel = thisRigidbody.velocity;
			vel.x = speed;
			thisRigidbody.velocity = vel;
		}
		if (gameController.Direction.Left.WasReleased) {
			Vector3 vel = thisRigidbody.velocity;
			vel.x = 0;
			thisRigidbody.velocity = vel;
		}
		if (gameController.Direction.Right.WasReleased) {
			Vector3 vel = thisRigidbody.velocity;
			vel.x = 0;
			thisRigidbody.velocity = vel;
		}
	}

	public void CreateBoss (int bossNum, Vector3 cameraPos){
		playerNum = bossNum;
		Vector3 pos = cameraPos;
		pos.y += 7;
		pos.z = 0;
		this.transform.position = pos;
	}
}
