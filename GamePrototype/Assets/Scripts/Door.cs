using UnityEngine;
using System.Collections;

/*
public struct DoubleVector {
	Vector3 p1;
	Vector3 p2;
};
*/

public class Door : MonoBehaviour {

	public Direction dir;
	int numPlayersEntered = 0;

	private BoxCollider bc;

	// Use this for initialization
	void Start () {
		bc = gameObject.AddComponent<BoxCollider>();
		bc.center = Vector3.zero;
		bc.size = new Vector3 (1.5f, 1.5f, 1.5f);
		bc.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		
		  //This code says that if all the players are in the door's vicinity,
		   //then move the camera and players to the next room
		if (numPlayersEntered == GameManager.S.numPlayers) {
			CameraController.S.TransitionCamera (dir);
			//PlayerTransition.S.MovePlayers (dir);
		}
	}

	void OnTriggerEnter(Collider other){
		print ("Player entered the room");
		if (other.CompareTag("Player")) {
			print ("Player tag present");
			++numPlayersEntered;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {
			--numPlayersEntered;
		}
	}

	/*
	 * Need to write some functions to handle player movement when the door is called
	 */
}
