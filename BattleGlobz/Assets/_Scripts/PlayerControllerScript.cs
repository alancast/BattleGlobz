using UnityEngine;
using System.Collections;
using InControl; 

public class PlayerControllerScript : MonoBehaviour {
	//instance of the Player that can be accessed from anywhere
	public static PlayerControllerScript	instance;
	//Rigidbody so that only need to call getComponent once
	public Rigidbody	thisRigidbody;
	//what will be shot when you fire the gun (set in inspector)
	public GameObject	projectile;
	//Players shield child object
	GameObject 		shield;
	bool 			shieldUp = false;
	float 			shieldEnergy;
	float 			maxShieldEnergy = 200f;
	//Players gun child object
	GameObject		gun;
	//accelerations for horizontal movement and jumping
	float				xAccel = 20;
	float				jumpAccel = 400;
	//maximum speeds (directionless) for horizontal and jumping
	float 				maxXSpeed = 20;
	float 				maxJumpSpeed = 10;
	//speed projectile moves at
	float				projectileSpeed = 20;
	//what player this is 1,2,3 or 4 (set in inspector)
	public int			playerNum;
	//set to true if you are testing game with keyboard
	public bool			testingWithKeyboard = false;
	//Handling death and respawning
	Vector3				respawnPoint;
	bool				isDead = false;
	float				timeOfDeath = 0f;
	float				deathTimer = 1f;
	
	void Awake(){
		instance = this;
		thisRigidbody = GetComponent<Rigidbody>();
		gun = transform.GetChild(0).gameObject;
		shield = transform.GetChild(1).GetChild(0).gameObject;
		shield.renderer.enabled = false;
		shield.collider.enabled = false;
		shieldEnergy = maxShieldEnergy;
		this.tag = "Player" + playerNum.ToString ();
		respawnPoint = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Use last device which provided input.
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;

		if (isDead && Time.time > timeOfDeath + deathTimer)
			handleRespawn ();
		handleVelocity();
		handleJumping();
		handleGunAndShield();
	}
	
	//controls the players velocity every update
	void handleVelocity(){
		//for testing with keyboard
//		--------------------------------------------------------------------------
		if (testingWithKeyboard){
			if (Input.GetKey(KeyCode.LeftArrow)){
				if (thisRigidbody.velocity.x > -maxXSpeed){
					thisRigidbody.AddForce(Vector3.left*xAccel);
				}
			}
			if (Input.GetKey(KeyCode.RightArrow)){
				if (thisRigidbody.velocity.x < maxXSpeed){
					thisRigidbody.AddForce(Vector3.right*xAccel);
				}
			}
		}
//		--------------------------------------------------------------------------
		//for testing with controller
		else {
			var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
			if (gameController.LeftStick.Left){
				if (thisRigidbody.velocity.x > -maxXSpeed){
					Vector3 forceVector = Vector3.zero;
					forceVector.x = gameController.LeftStickX;
					thisRigidbody.AddForce(forceVector*xAccel);
				}
			}
			if (gameController.LeftStick.Right){
				if (thisRigidbody.velocity.x < maxXSpeed){
					Vector3 forceVector = Vector3.zero;
					forceVector.x = gameController.LeftStickX;
					thisRigidbody.AddForce(forceVector*xAccel);
				}
			}
		}
	}
	
	//controls the players jumping every update
	void handleJumping(){
		//for testing with keyboard
//		--------------------------------------------------------------------------
		if (testingWithKeyboard){
			if (Input.GetKeyDown(KeyCode.UpArrow)){
				if (thisRigidbody.velocity.y < maxJumpSpeed){
					thisRigidbody.AddForce(Vector3.up*jumpAccel);
				}
			}
		}
//		--------------------------------------------------------------------------
		//for testing with controller
		else {
			var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
			if (gameController.RightBumper.WasPressed || gameController.LeftBumper.WasPressed){
				if (thisRigidbody.velocity.y < maxJumpSpeed){
					thisRigidbody.AddForce(Vector3.up*jumpAccel);
				}
			}
		}
	}
		
	//handles the gun and shield every update, rotation and shooting
	void handleGunAndShield(){
		//for testing with keyboard
//		--------------------------------------------------------------------------
		if (testingWithKeyboard){
			if (Input.GetKeyDown(KeyCode.Space)){
				Vector3 projectileVelocity = shootAngle();
				shootProjectile(projectileVelocity * projectileSpeed);
			}
			// Need to implement for shield
		}
//		--------------------------------------------------------------------------
		//for testing with controller
		else {
			var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
			//fire gun
			if (gameController.RightTrigger.WasPressed){
				Vector3 projectileVelocity = shootAngle();
				shootProjectile(projectileVelocity * projectileSpeed);
			}
			// generate shield
			if (gameController.LeftTrigger.WasPressed) { 
				print ("shield UP");
				shieldUp = true;
				shield.renderer.enabled = true;
				shield.collider.enabled = true;

			}
			// remove shield
			if (gameController.LeftTrigger.WasReleased) {
				print ("shield DOWN");
				shieldUp = false;
				shield.renderer.enabled = false;
				shield.collider.enabled = false;
			}
			//rotate gun and shield
			if (rightStickPressed()){
				float rot = rotationAmount();
				gun.transform.RotateAround(transform.position, Vector3.forward, rot);
				shield.transform.RotateAround(transform.position, Vector3.forward, rot);
			}
		}

		// Update shield energy and size
		if (shieldUp) {
			shieldEnergy -= 100f * Time.deltaTime;
			if(shieldEnergy < 0f){
				shieldEnergy = 0f;
				print ("shield DOWN");
				shieldUp = false;
				shield.renderer.enabled = false;
				shield.collider.enabled = false;
			}
		} else {
			shieldEnergy += 100f * Time.deltaTime;
			if(shieldEnergy > maxShieldEnergy){
				shieldEnergy = maxShieldEnergy;
			}
		}
		print (shieldEnergy);
		shield.transform.localScale = new Vector3(0.3f, 2f * shieldEnergy/maxShieldEnergy, 1f);
	}
	
	//will instantiate a projectile with initial velocity "velocity" passed in
	void shootProjectile(Vector3 velocity){
		GameObject temp = (GameObject)Instantiate(projectile, transform.position, 
		                                          Quaternion.Euler(Vector3.zero));
		temp.rigidbody.velocity = velocity;
		temp.tag = "Bullet" + playerNum.ToString ();
	}
	
	//checks if the right stick is pressed over an assigned threshold
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
	
	//returns the angle the projectile will be shot at
	//depends on current rotation of gun
	Vector3 shootAngle(){
		Vector3 retVec = Vector3.zero;
		float z = gun.transform.eulerAngles.z;
		retVec.x = Mathf.Cos(z * Mathf.Deg2Rad);
		retVec.y = Mathf.Sin(z * Mathf.Deg2Rad);
		return retVec;
	}

	public void handleDeath(){
		this.transform.position = new Vector3 (-100, -100, 0);
		timeOfDeath = Time.time;
		isDead = true;

	}

	void handleRespawn(){
		thisRigidbody.velocity = new Vector3(0f,0f,0f);
		this.transform.position = respawnPoint;
		isDead = false;

	}
}
	