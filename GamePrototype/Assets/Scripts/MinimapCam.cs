using UnityEngine;
using System.Collections;

public class MinimapCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 pos = new Vector3 (110, -30, -100);
		gameObject.transform.position = pos;
	}

	/*
	// Update is called once per frame
	void Update () {
		Vector3 pos = Camera.main.transform.position;

		pos.x += 35;
		pos.y += 50;
		gameObject.transform.position = pos;
	}
	*/
}
