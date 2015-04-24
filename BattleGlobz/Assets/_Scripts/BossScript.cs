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
	
	//Animators
	public Animator 	wings;
	public Animator		head;
	public Animator		gem;
	public Animator 	cutScene;
	public Animator		healthBar;

	float				cutsceneLength = 2f;


	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		gun = transform.GetChild(0).gameObject;
		coolDownSphere = transform.GetChild (1).gameObject;
	}

	void OnTriggerEnter(Collider other){

		print ("hit");

		if (other.tag == "Projectile") {
			ProjectileScript projectile = other.gameObject.GetComponent<ProjectileScript>();
			if(projectile.ownerNum != playerNum && projectile.ownerNum != -1){
				health--;

				//handle boss Hit Anim
				head.SetTrigger("Hit");
				print ("hit");
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		handleAnims ();


		if (health <= 0) {
			handleBossDeath();
		}
		if (playerNum == -1){
			return;
		}
		//to stop shooting without releasing the trigger
		if(Time.timeSinceLevelLoad > stopShootingTime && !onCoolDown){
			coolDownTime = Time.timeSinceLevelLoad;
			onCoolDown = true;
			coolDownLength = (Time.timeSinceLevelLoad - startShootingTime);
			isShooting = false;
			stopShootingTime += 100000;
			Vector3 vel = thisRigidbody.velocity;
			vel.x = 0;
			thisRigidbody.velocity = vel;
		}
		if((!isShooting || Time.timeSinceLevelLoad < stopShootingTime) && !onCoolDown){
			handleMovement ();
			handleShooting();
		}
		if (onCoolDown) {
			float ratio = (coolDownLength - (Time.timeSinceLevelLoad - coolDownTime))/2;
			coolDownSphere.transform.localScale = new Vector3(ratio, ratio, 1.1f);
		}
		if (Time.timeSinceLevelLoad > coolDownTime + coolDownLength && onCoolDown){
			coolDownSphere.transform.localScale = new Vector3(0f,0f,1.1f);
			onCoolDown = false;
		}
		
		if (CameraScript.playerCountAlive == 0 && Time.timeSinceLevelLoad < endGameTime){
			CameraScript.instance.timeText.fontSize = 50;
			CameraScript.instance.timeText.text = "Player " + playerNum.ToString() + System.Environment.NewLine + "WINS";
			endGameTime = Time.timeSinceLevelLoad;
		}
		
		if (Time.timeSinceLevelLoad > endGameTime + endGamePause){
//			Application.LoadLevel("_Scene_Menu");
			CameraScript.instance.timeText.text = "Refresh";
		}
	}

	void handleAnims (){

		healthBar.SetInteger ("Health", health);

		head.SetFloat("x_vel", this.thisRigidbody.velocity.x);

		float x_vel_abs = (float) Mathf.Abs(this.thisRigidbody.velocity.x);
		wings.SetFloat ("x_vel", x_vel_abs);


		//handle shooting anim for head
		head.SetBool("Shooting", isShooting);
		gem.SetBool ("Shooting", isShooting);
	}

	void handleShooting(){
		var gameController = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (gameController.RightTrigger.IsPressed) {
			if (!isShooting){
				stopShootingTime = Time.timeSinceLevelLoad + shootingLength;
				startShootingTime = Time.timeSinceLevelLoad;
			}
			thisRigidbody.velocity = Vector3.zero;
			isShooting = true;
			//Handle coolDownSphere size
			float elapsed = 1 - (stopShootingTime - Time.timeSinceLevelLoad);
			float ratio = (elapsed/shootingLength)/2;
			coolDownSphere.transform.localScale = new Vector3(ratio, ratio, 1.1f);

			if (Time.timeSinceLevelLoad > nextShotTime){
				Vector3 projectileVelocity = shootAngle();
				shootProjectile(projectileVelocity * bossBulletSpeed);
				nextShotTime = Time.timeSinceLevelLoad + shootFrequency;
			}
		}
		if (gameController.RightTrigger.WasReleased) {
			coolDownTime = Time.timeSinceLevelLoad;
			onCoolDown = true;
			coolDownLength = (Time.timeSinceLevelLoad - startShootingTime);
			isShooting = false;
			stopShootingTime += 100000;
			Vector3 vel = thisRigidbody.velocity;
			vel.x = 0;
			thisRigidbody.velocity = vel;
		}
		if (rightStickPressed()){
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

	public void triggerCutScene(int playerNum){
		cutScene.SetInteger ("PlayerNum", playerNum);
		cutScene.SetTrigger ("StartCutScene");
	}

	public void CreateBoss (int bossNum, Vector3 cameraPos, Material mat){

		playerNum = bossNum;
		Vector3 pos = cameraPos;
		//this.GetComponent<Renderer> ().material = mat;
		pos.y = -2;
		pos.z = 0;
		transform.position = pos;
		print (pos.y);
		//CameraScript.instance.source.PlayOneShot(CameraScript.instance.bossMode);

		PlayerControllerScript[] players = FindObjectsOfType<PlayerControllerScript> ();
		foreach(PlayerControllerScript p in players){
			p.startCutscene(cutsceneLength);
			if(p.playerNum == bossNum){
				p.bottomFace.SetBool("boss", true);
			}
		}

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
		Vector3 GemPos = transform.position + shootAngle ().normalized * 4.5f;
		GameObject temp = (GameObject)Instantiate(projectile, GemPos, Quaternion.Euler(Vector3.zero));
		temp.GetComponent<Rigidbody>().velocity = velocity;
		temp.GetComponent<BossBulletScript> ().ownerNum = playerNum;
		temp.GetComponent<BossBulletScript> ().throwAt = Time.timeSinceLevelLoad;
	}

	void handleBossDeath(){

		PlayerControllerScript[] players = FindObjectsOfType<PlayerControllerScript> ();
		foreach(PlayerControllerScript p in players){
			if(p.playerNum == playerNum){
				p.bottomFace.SetTrigger("gone");
			}
		}

		CameraScript.isBoss = false;
		Vector3 crownPos = transform.position;
		crownPos.y -= 3f; 
		GameObject temp = (GameObject) Instantiate (crownPrefab, crownPos, Quaternion.Euler (Vector3.zero));
		transform.position = new Vector3 (0, -200, 0);
		health = maxHealth;
		playerNum = -1;
		//decrement number of players in game because whoever was boss is out of the game
		CameraScript.playerCount--;
		CameraScript.playerCountAlive = CameraScript.playerCount;
		onCoolDown = false;
		coolDownTime = 0;
		isShooting = false;
	}
}
