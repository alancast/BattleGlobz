using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		switch (other.gameObject.tag) {
		case "Player0":
			if (this.tag == "Goal1")
				return;
			else
				Application.LoadLevel ("_Scene_Connor_0");
			break;
		case "Player1":
			if (this.tag == "Goal0")
				return;
			else
				Application.LoadLevel ("_Scene_Connor_0");
			break;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
