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

	/*
	void OnTriggerEnter2D(Collider2D other){
		if(other.CompareTag("Player")){
		++numPlayers;
			if (numPlayers == GameManager.S.numPlayers) {
				door.locked = false;
				//otherSide.door.locked = true; //Don't think I need to be handling this here

				//TODO Transition camera to be looking at the entire hallway plus 
				//the other room that it isn't currently looking at
				//TODO Set the camera to MY ROOM and MY DOOR and MY SIDE OF THE HALLWAY
				//CameraController.S.TransitionCamera(Direction.None, true);
				Vector3 pos = gameObject.transform.parent.transform.position; //parent hallway position
				bool offset = (pos != new Vector3 (pos.x + gameObject.transform.position.x, pos.y + gameObject.transform.position.y, pos.z));
				CameraController.S.PutCameraInHallway (dir, gameObject.transform.parent.transform.position, offset);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.CompareTag ("Player")) {
			--numPlayers;
			if (!door.locked) {
				door.locked = true;
			}
		}
	}

	Vector3 GetHallwayOffsetPosition(){
		Vector3 hallwayPos = gameObject.transform.parent.transform.position;
		if (dir == Direction.Right) {
			hallwayPos = new Vector3 (hallwayPos.x + gameObject.transform.position.x, hallwayPos.y, 0);
		} else {
			hallwayPos = new Vector3 (hallwayPos.x, hallwayPos.y + gameObject.transform.position.y, 0);
		}
		return hallwayPos;
	}
	*/

}
