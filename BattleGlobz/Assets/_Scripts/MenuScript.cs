using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha2))
			Application.LoadLevel ("_Scene_2Player");
		else if (Input.GetKeyDown (KeyCode.Alpha3))
			Application.LoadLevel ("_Scene_3Player");
		else if(Input.GetKeyDown (KeyCode.Alpha4))
			Application.LoadLevel ("_Scene_4Player");
	}
}
