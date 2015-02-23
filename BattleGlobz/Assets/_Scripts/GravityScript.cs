using UnityEngine;
using System.Collections;

public class GravityScript : MonoBehaviour {

	public Vector3 newGravity = new Vector3(0, -9.8f, 0);

	// Use this for initialization
	void Start () {
		Physics.gravity = newGravity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
