using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.S.playersInitialized) {
			Vector3 p1Pos = GameManager.S.players [0].transform.position;
			Vector3 p2Pos = GameManager.S.players [1].transform.position;

			Vector3 cameraPos = Vector3.Lerp (p1Pos, p2Pos, .5f);
			cameraPos.z = -10;
			gameObject.transform.position = cameraPos;
		}
	}
}
