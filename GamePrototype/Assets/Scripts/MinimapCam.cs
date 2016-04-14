using UnityEngine;
using System.Collections;

public class MinimapCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = Camera.main.transform.position;
	}
}
