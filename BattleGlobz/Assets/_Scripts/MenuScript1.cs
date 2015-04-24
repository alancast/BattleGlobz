using UnityEngine;
using System.Collections;
using InControl;

public class MenuScript1 : MonoBehaviour {
	public SpriteRenderer controlsSprite;
	public SpriteRenderer dpadSprite;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		var gameController = InputManager.ActiveDevice;
		

		if (gameController.DPadUp.WasPressed){
			controlsSprite.enabled = true;
			Color newC = dpadSprite.color;
			newC.a = 0f;
			dpadSprite.color = newC;

		} else if (gameController.DPadUp.WasReleased){
			controlsSprite.enabled = false;
			Color newC = dpadSprite.color;
			newC.a = 1f;
			dpadSprite.color = newC;

		}

		if (gameController.DPadLeft.WasPressed){
			Application.LoadLevel("_Scene_2Player");

		}
		if (gameController.DPadDown.WasPressed){
			Application.LoadLevel("_Scene_3Player");

		}
		if (gameController.DPadRight.WasPressed){
			Application.LoadLevel("_Scene_4Player");

		}


	}
}
