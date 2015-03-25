using UnityEngine;
using System.Collections;

public class GravityController : MonoBehaviour {

	Vector3 newGravity = new Vector3(0, -80f, 0);

	// Use this for initialization
	void Start () {
		Physics.gravity = newGravity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
