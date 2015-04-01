using UnityEngine;
using System.Collections;
using InControl;

public class BossScript : MonoBehaviour {

	//Rigidbody so that only need to call getComponent once
	public Rigidbody	thisRigidbody;
	int					playerNum = -1;
	int					maxHealth = 3;
	int					health = 3;
	float				speed = 8;
	GameObject			gun;
	GameObject			coolDownSphere;
	//what will be shot when fired (set in inspector)
	public GameObject			crownPrefab;
	public GameObject			projectile;
	public GameObject			ballPrefab;
	bool				isShooting = false;
	//how long a boss can shoot for
	float				shootingLength = 1;
	//when a boss must stop shooting
	float 				stopShootingTime = 1000000;
	//how frequently you can shoot
	float 				shootFrequency = .04f;
	float				startShootingTime = 0f;
	//when you can shoot the next shot
	float				nextShotTime = 0;
	bool				onCoolDown;
	float				coolDownLength = 1f;
	float				coolDownTime = 0f;
	float				coolDownRatio = 1.5f;
	//speed of the boss bullet when fired
	float				bossBulletSpeed = 30;
	//the menu screen will load when this is over
	float				endGameTime = 100000000;
	//how long the game will pause after there is a winner
	float				endGamePause = 5;

	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		gun = transform.GetChild(0).gameObject;
		coolDownSphere = transform.GetChild (1).gameObject;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Projectile") {
			ProjectileScript projectile = other.gameObject.GetComponent<ProjectileScript>();
			if(projectile.ownerNum != playerNum && projectile.ownerNum != -1){
				health--;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0) {
			handleBossDeath();
		}
		if (playerNum == -1)
			return;
		if(!isShooting && !onCoolDown)
			handleMovement ();
		handleShooting();

		if (onCoolDown) {
			float elapsed = coolDownLength - (Time.time - coolDownTime);
			float ratio = elapsed;
			coolDownSphere.transform.localScale = new Vector3(ratio, ratio, 1.1f);
		}

		if (Time.time > coolDownTime + coolDownLength && onCoolDown){
			coolDownSphere.transform.localScale = new Vector3(0f,0f,1.1f);
			onCoolDown = false;
		}
		
		if (CameraScript.playerCountAlive == 0 && Time.timeSinceLevelLoad < endGameTime){
			CameraScript.instance.timeText.text = "Champion is Player" + playerNum.ToString() + "!!!";
			endGameTime = Time.timeSinceLevelLoad;
		}
		
		if (Time.timeSinceLevelLoad > endGameTime + endGamePause){
//			Application.LoadLevel("_Scene_Menu");
			CameraScript.instance.timeText.text = "Refresh browser to play again";
		}
	}

	void handleShooting(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (gameController.RightTrigger.IsPressed && !onCoolDown && 
			(!isShooting || Time.time < stopShootingTime)) {
			if (!isShooting){
				stopShootingTime = Time.time + shootingLength;
				startShootingTime = Time.time;
			}
			thisRigidbody.velocity = Vector3.zero;
			isShooting = true;
			//Handle coolDownSphere size
			float elapsed = 1 - (stopShootingTime - Time.time);
			float ratio = elapsed/shootingLength;
			coolDownSphere.transform.localScale = new Vector3(ratio, ratio, 1.1f);

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
			coolDownLength = (Time.time - startShootingTime);

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

	public void CreateBoss (int bossNum, Vector3 cameraPos, Material mat){
		playerNum = bossNum;
		Vector3 pos = cameraPos;
		this.GetComponent<Renderer> ().material = mat;
		pos.y = -2;
		pos.z = 0;
		this.transform.position = pos;
		//CameraScript.instance.source.PlayOneShot(CameraScript.instance.bossMode);
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

	void handleBossDeath(){
		CameraScript.isBoss = false;
		GameObject temp = (GameObject) Instantiate (crownPrefab, transform.position, Quaternion.Euler (Vector3.zero));
		transform.position = new Vector3 (-100, -100, 0);
		health = maxHealth;
		playerNum = -1;
		//decrement number of players in game because whoever was boss is out of the game
		CameraScript.playerCount--;
		CameraScript.playerCountAlive = CameraScript.playerCount;
	}
}
