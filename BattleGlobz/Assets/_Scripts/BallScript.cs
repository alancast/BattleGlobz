using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y <= -50) {
			Vector3 newPos = Camera.current.transform.position;
			newPos.z = 0;
			this.transform.position = newPos;
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		}
	}
}
