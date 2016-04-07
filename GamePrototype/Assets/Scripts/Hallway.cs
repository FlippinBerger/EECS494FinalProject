using UnityEngine;
using System.Collections;

//This class handles each half of the hallways to transition the camera
public class Hallway : MonoBehaviour {

	private int numPlayers = 0; //number of players that are currently in this hallway
	public Door door;  //The door associated with this half of the hallway
	public Direction dir;

	//public Hallway otherSide; //The other half of the hallway. Not sure if necessary yet
		
	/*
	 * These Trigger functions handle what doors the players can currently utilize.
	 * The camera functionality is going to be handled only by the doors
	 * 
	 * These can possibly spawn the next room, however, the door might be a better place
	 * for spawning rooms as well
	 * */


	void OnTriggerEnter2D(Collider2D other){
		if(other.CompareTag("Player")){
			++numPlayers;
			if (numPlayers == GameManager.S.numPlayers) {
				
				if (CameraController.S.fromRoom) { //prevents the double camera transition when entering a hallway from a room
					CameraController.S.fromRoom = false;
					return;
				}
				//TODO door code needed
				//door.locked = false;
				//otherSide.door.locked = true; //Don't think I need to be handling this here

				CameraController.S.PutCameraInHallway (dir);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.CompareTag ("Player")) {
			print ("Exiting a hall trigger");
			--numPlayers;
			/*if (!door.locked) {
				door.locked = true;
			}*/
		}
	}
}
