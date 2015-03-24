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
	float			shieldSize;
	//Players gun child object
	GameObject		gun;
	//accelerations for horizontal movement and jumping
	float				xAccel = 8;
	float				jumpAccel = 1.8f;
	//maximum speeds (directionless) for horizontal and jumping
	float 				maxXSpeed = 15;
	float 				maxJumpSpeed = 30;

	//speed projectile moves at
	float				projectileSpeed = 60;
	//placeholder for time when the ball needs to be fired
	float				ballFireTime = 10000000000;
	//amount of time you can hold onto ball for
	float				ballHoldTime = 3;
	//what player this is 1,2,3 or 4 (set in inspector)
	public int			playerNum;
	//set to true if you are testing game with keyboard

	public bool			testingWithKeyboard = false;
	//Handling death and respawning
	Vector3				respawnPoint;
	bool				isDead = false;
	float				timeOfDeath = 0f;
	float				deathTimer = 1f;
	bool 				hasProjectile = false;
	GameObject			ballInd;
	
	//tells whether the player is on the ground or not. set in isGrounded() called on update
	bool				grounded = true;
	//tells whether or not the player is dashing 
	bool				isDashing = false;
	bool				canDash = true;
	float				dashTime = 0f;
	float				dashLength = .17f;
	float 				dashSpeed = 2f;
	float 				dashResistance = -0.4f;
	Vector3				dashForce = Vector3.zero;
	//how many hits it takes to die
	int					maxHealth = 1;
	//seperate from MaxHealth so that numbers aren't hard coded anywhere else in code
	int					currentHealth = 1;
	
	void Awake(){
		instance = this;
		thisRigidbody = GetComponent<Rigidbody>();
		gun = transform.GetChild(0).gameObject;
		shield = transform.GetChild(1).gameObject;
		ballInd = transform.GetChild(0).transform.GetChild(0).gameObject;
		shield.GetComponent<Renderer>().enabled = false;
		shield.GetComponent<Collider>().enabled = false;
		shieldEnergy = maxShieldEnergy;
		respawnPoint = new Vector3 (Camera.main.transform.position.x, -6, 0);
		shieldSize = shield.transform.lossyScale.y;
		ballInd.GetComponent<Renderer>().enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		// Use last device which provided input.
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;

		if (isDead && Time.time > timeOfDeath + deathTimer)
			handleRespawn ();

		isGrounded ();
		handleVelocity();
		handleJumping();
		handleGunAndShield();
		handleDash ();
	}

	void handleDash() {
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		if (isDashing) {
			thisRigidbody.AddForce(thisRigidbody.velocity*Time.deltaTime*dashResistance);
		}
		if(!isDashing && canDash && !grounded && gameController.RightBumper.WasPressed){
			isDashing = true;
			canDash = false;
			dashTime = Time.time;
			dashForce = Vector3.zero;
			dashForce.x = dashSpeed * gameController.LeftStickX;
			dashForce.y = dashSpeed * gameController.LeftStickY;
			thisRigidbody.AddForce(dashForce);
		}
		if (Time.time > dashTime + dashLength && isDashing) {
			isDashing = false;
		}

	}

	void isGrounded(){
		Vector3 leftOrigin = thisRigidbody.transform.position;
		leftOrigin.x -= GetComponent<Collider> ().bounds.size.x/2;
		Vector3 rightOrigin = thisRigidbody.transform.position;
		rightOrigin.x += GetComponent<Collider> ().bounds.size.x/2;
		if (Physics.Raycast (leftOrigin, Vector3.down, GetComponent<Collider> ().bounds.size.y/2 + .05f)
		    || Physics.Raycast (rightOrigin, Vector3.down, GetComponent<Collider> ().bounds.size.y/2 + .05f)) {
			if(!grounded)
				canDash = true;
			grounded = true;
		}
		else
			grounded = false;
	}

	//controls the players velocity every update
	void handleVelocity(){
		//for testing with keyboard
//		--------------------------------------------------------------------------
		if (testingWithKeyboard){
			if (Input.GetKey(KeyCode.LeftArrow)){
				if (thisRigidbody.velocity.x > -maxXSpeed){
					thisRigidbody.AddForce(Vector3.left*xAccel*Time.deltaTime);
				}
			}
			if (Input.GetKey(KeyCode.RightArrow)){
				if (thisRigidbody.velocity.x < maxXSpeed){
					thisRigidbody.AddForce(Vector3.right*xAccel*Time.deltaTime);
				}
			}
		}
//		--------------------------------------------------------------------------
		//for testing with controller
		else {
			var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
			if (gameController.LeftStick.Left){
				if (thisRigidbody.velocity.x > -maxXSpeed && !isDashing){
					Vector3 forceVector = Vector3.zero;
					forceVector.x = gameController.LeftStickX;
					thisRigidbody.AddForce(forceVector*xAccel*Time.deltaTime);
				}
			}
			if (gameController.LeftStick.Right){
				if (thisRigidbody.velocity.x < maxXSpeed && !isDashing){
					Vector3 forceVector = Vector3.zero;
					forceVector.x = gameController.LeftStickX;
					thisRigidbody.AddForce(forceVector*xAccel*Time.deltaTime);
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
				if (thisRigidbody.velocity.y < maxJumpSpeed && grounded){
					thisRigidbody.AddForce(Vector3.up*jumpAccel);
				}
			}
		}
//		--------------------------------------------------------------------------
		//for testing with controller
		else {
			var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
			if (gameController.RightBumper.WasPressed || gameController.LeftBumper.WasPressed){
				if (thisRigidbody.velocity.y < maxJumpSpeed && grounded){
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
			if (Input.GetKeyDown(KeyCode.Space) && hasProjectile && !shieldUp){
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
			if ((gameController.RightTrigger.WasPressed && hasProjectile && !shieldUp)
				|| (Time.time > ballFireTime)){
				Vector3 projectileVelocity = shootAngle();
				shootProjectile(projectileVelocity * projectileSpeed);
				hasProjectile = false;
				ballFireTime += 10000;
			}
			// generate shield
			if (gameController.LeftTrigger.WasPressed) { 
				shieldUp = true;
				shield.GetComponent<Renderer>().enabled = true;
				shield.GetComponent<Collider>().enabled = true;

			}
			// remove shield
			if (gameController.LeftTrigger.WasReleased) {
				shieldUp = false;
				shield.GetComponent<Renderer>().enabled = false;
				shield.GetComponent<Collider>().enabled = false;
			}
			//rotate gun and shield
			if (rightStickPressed()){
				float rot = rotationAmount();
				gun.transform.RotateAround(transform.position, Vector3.forward, rot);
				shield.transform.RotateAround(transform.position, Vector3.forward, rot);
			}
		}

		// Update shield energy and size
		//smaller numbers = slower charge
		float shieldChargeRate = 50f;
		if (shieldUp) {
			shieldEnergy -= shieldChargeRate * Time.deltaTime;
			if(shieldEnergy < 0f){
				shieldEnergy = 0f;
				shieldUp = false;
				shield.GetComponent<Renderer>().enabled = false;
				shield.GetComponent<Collider>().enabled = false;
			}
		} else {
			shieldEnergy += shieldChargeRate * Time.deltaTime;
			if(shieldEnergy > maxShieldEnergy){
				shieldEnergy = maxShieldEnergy;
			}
		}
		shield.transform.localScale = new Vector3(0.3f, shieldSize * shieldEnergy/maxShieldEnergy, 1f);
	}
	
	//will instantiate a projectile with initial velocity "velocity" passed in
	void shootProjectile(Vector3 velocity){
		ballInd.GetComponent<Renderer>().enabled = false;
		GameObject temp = (GameObject)Instantiate(projectile, transform.position, Quaternion.Euler(Vector3.zero));
		temp.GetComponent<Rigidbody>().velocity = velocity;
		temp.GetComponent<ProjectileScript> ().ownerNum = playerNum;
		temp.GetComponent<ProjectileScript> ().throwAt = Time.time;
		temp.GetComponent<Renderer> ().material = this.GetComponent<Renderer> ().material;
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

	//int killerNum is the number of the player who made the kill
	public void handleDeath(int killerNum){
		if (currentHealth != 1){
			currentHealth--;
			return;
		}
		currentHealth = maxHealth;
		this.transform.position = new Vector3 (-100, -100, 0);
		timeOfDeath = Time.time;
		isDead = true;
		CameraScript.instance.addScore(killerNum,1);
	}

	void handleRespawn(){
		thisRigidbody.velocity = new Vector3(0f,0f,0f);
		this.transform.position = new Vector3 (Camera.main.transform.position.x, -6, 0);
		isDead = false;
	}

	public void pickUpProjectile(){
		ballInd.GetComponent<Renderer>().enabled = true;
		hasProjectile = true;
		ballFireTime = Time.time + ballHoldTime;
	}

	public bool hasBall(){
		return hasProjectile;
	}
}
	