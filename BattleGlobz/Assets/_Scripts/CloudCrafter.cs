using UnityEngine;
using System.Collections;

public class CloudCrafter : MonoBehaviour {
	
	public bool hideGizmos = false;
	
	// fields set in the Unity Inspector pane
	public int numClouds = 40; // The # of clouds to make
	public GameObject[] cloudPrefabs;
	Vector3 cloudPosCenter;
	public Vector3 cloudPosSize;
	public float cloudScaleMin = 1;
	public float cloudScaleMax = 5;
	public float cloudSpeedMult = .5f;
	public bool _______________;
	
	// fields set dynamically
	public GameObject[] cloudInstances;
	
	void Awake(){
		cloudPosCenter = transform.position;
		cloudPosSize = transform.localScale;
		// Make an array large enough to hold all the Cloud_ instances 
		cloudInstances = new GameObject[numClouds];
		// Find the CloudAnchor parent GameObject
		// Iterate through and make Cloud_s
		GameObject cloud;
		for (int i=0; i<numClouds; i++) {
			// Pick an int between 0 and cloudPrefabs.Length-1
			// Random.Range will not ever pick as high as the top number 
			int prefabNum = Random.Range(0,cloudPrefabs.Length);
			// Make an instance
			cloud = Instantiate( cloudPrefabs[prefabNum] ) as GameObject; 
			// Position cloud
			Vector3 cPos = Vector3.zero;
			cPos.x = Random.Range( cloudPosCenter.x - cloudPosSize.x / 2, 
			                      cloudPosCenter.x + cloudPosSize.x / 2); 
			// Scale cloud
			float scaleU = Random.value;
			float scaleVal = Mathf.Lerp( cloudScaleMin, cloudScaleMax, scaleU);
			// Smaller clouds (with smaller scaleU) should be nearer the ground
			cPos.y = Mathf.Lerp( cloudPosCenter.y - cloudPosSize.y / 2, 
			                    cloudPosCenter.y + cloudPosSize.y / 2, scaleU + Random.value / 10 ); 
			// Smaller clouds should be further away
			cPos.z = 100 - 90*scaleU;
			// Apply these transforms to the cloud 
			cloud.transform.position = cPos; 
			cloud.transform.localScale = Vector3.one * scaleVal; 
			// Make cloud a child of the anchor 
			cloud.transform.parent = transform;
			// Add the cloud to cloudInstances
			cloudInstances[i] = cloud;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Iterate over each cloud that was created 
		foreach (GameObject cloud in cloudInstances) {
			// Get the cloud scale and position
			float scaleVal = cloud.transform.localScale.x;
			Vector3 cPos = cloud.transform.position;
			// Move larger clouds faster
			cPos.x -= scaleVal * Time.deltaTime * cloudSpeedMult; 
			// If a cloud has moved too far to the left...
			if (cPos.x <= cloudPosCenter.x - cloudPosSize.x / 2) {
				// Move it to the far right
				cPos.x = cloudPosCenter.x + cloudPosSize.x /2 ; 
			}
			// Apply the new position to cloud
			cloud.transform.position = cPos; 
		}
	}
	
	void OnDrawGizmos(){
		if (hideGizmos) return;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(transform.position, transform.localScale);
	}
}
