using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	static public CameraController S;

	public float horizontalOffset = 13.5f;
	public float verticalOffset = 7.5f;

	void Awake(){
		S = this;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Places the camera in accordance to the provided room vector
	public void SetCameraPosition(Vector3 roomPos){
		Camera.main.transform.position = new Vector3 (roomPos.x + horizontalOffset, roomPos.y + verticalOffset, -10f);
	}

	public void TransitionCamera(Direction d){
		int roomHeight = GameManager.S.roomHeight;
		int roomWidth = GameManager.S.roomWidth;

		Vector3 pos = gameObject.transform.position;
		print (pos);

		switch (d) {
		case Direction.Up:
			gameObject.transform.position = new Vector3 (pos.x, pos.y + roomHeight, pos.z); 
			break;
		case Direction.Down:
			gameObject.transform.position = new Vector3 (pos.x, pos.y - roomHeight, pos.z); 
			break;
		case Direction.Left:
			gameObject.transform.position = new Vector3 (pos.x - roomWidth, pos.y, pos.z);
			break;
		case Direction.Right:
			gameObject.transform.position = new Vector3 (pos.x + roomWidth, pos.y, pos.z);
			break;
		}
	}
}
