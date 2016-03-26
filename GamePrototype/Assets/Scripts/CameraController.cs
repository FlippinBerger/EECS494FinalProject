using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	static public CameraController S;

	public float horizontalOffset = 13.5f;
	public float verticalOffset = 7.5f;

	// Use this for initialization
	void Start () {
		S = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Places the camera in accordance to the provided room vector
	public void SetCameraPosition(Vector3 roomPos){
		Camera.main.transform.position = new Vector3 (roomPos.x + horizontalOffset, roomPos.y + verticalOffset, -10f);
	}
}
