using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public Direction dir;
	int numPlayersClose = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		  //This code says that if all the players are in the door's vicinity,
		   //then move the camera and players to the next room
		if (numPlayersClose == GameManager.S.numPlayers) {
			GameManager.S.MoveCamera (dir);
			//PlayerTransition.S.MovePlayers (dir);
		}

	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag("Player")) {
			++numPlayersClose;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {
			--numPlayersClose;
		}
	}
}
