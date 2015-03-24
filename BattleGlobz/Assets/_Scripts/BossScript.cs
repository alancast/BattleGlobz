using UnityEngine;
using System.Collections;
using InControl;

public class BossScript : MonoBehaviour {

	//Rigidbody so that only need to call getComponent once
	public Rigidbody	thisRigidbody;
	int					playerNum = -1;
	int					health = 10;
	float				speed = 5;
	GameObject			gun;
	//what will be shot when fired (set in inspector)
	public GameObject			projectile;
	public GameObject			ballPrefab;
	bool				isShooting = false;
	//how long a boss can shoot for
	float				shootingLength = 1;
	//when a boss must stop shooting
	float 				stopShootingTime = 1000000;
	//how frequently you can shoot
	float 				shootFrequency = .04f;
	//when you can shoot the next shot
	float				nextShotTime = 0;
	bool				onCoolDown;
	float				coolDownLength = 2f;
	float				coolDownTime = 0f;
	//speed of the boss bullet when fired
	float				bossBulletSpeed = 20;

	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		gun = transform.GetChild(0).gameObject;
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
			Destroy (this.gameObject);
		}
		if (playerNum == -1)
			return;
		if(!isShooting && !onCoolDown)
			handleMovement ();
		handleShooting();
		if (Time.time > coolDownTime + coolDownLength){
			onCoolDown = false;
		}
	}

	void handleShooting(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (gameController.RightTrigger.IsPressed && !onCoolDown && 
			(!isShooting || Time.time < stopShootingTime)) {
			if (!isShooting){
				stopShootingTime = Time.time + shootingLength;
			}
			thisRigidbody.velocity = Vector3.zero;
			isShooting = true;
			if (Time.time > nextShotTime){
				Vector3 projectileVelocity = shootAngle();
				shootProjectile(projectileVelocity * bossBulletSpeed);
				nextShotTime = Time.time + shootFrequency;
			}
		}
		if ((gameController.RightTrigger.WasReleased && !onCoolDown)
			|| (Time.time > stopShootingTime && !onCoolDown)) {
			coolDownTime = Time.time;
			onCoolDown = true;
			isShooting = false;
			stopShootingTime += 100000;
		}

		if (rightStickPressed() && !onCoolDown){
			float rot = rotationAmount();
			gun.transform.RotateAround(transform.position, Vector3.forward, rot);
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
		Instantiate (ballPrefab, cameraPos, Quaternion.Euler(Vector3.zero));
		pos.y += 7;
		pos.z = 0;
		this.transform.position = pos;
		CameraScript.instance.source.PlayOneShot(CameraScript.instance.bossMode);
	}
	
	Vector3 shootAngle(){
		Vector3 retVec = Vector3.zero;
		float z = gun.transform.eulerAngles.z;
		retVec.x = Mathf.Cos(z * Mathf.Deg2Rad);
		retVec.y = Mathf.Sin(z * Mathf.Deg2Rad);
		return retVec;
	}
	
	//will instantiate a projectile with initial velocity "velocity" passed in
	void shootProjectile(Vector3 velocity){
		GameObject temp = (GameObject)Instantiate(projectile, transform.position, Quaternion.Euler(Vector3.zero));
		temp.GetComponent<Rigidbody>().velocity = velocity;
		temp.GetComponent<BossBulletScript> ().ownerNum = playerNum;
		temp.GetComponent<BossBulletScript> ().throwAt = Time.time;
	}
}
