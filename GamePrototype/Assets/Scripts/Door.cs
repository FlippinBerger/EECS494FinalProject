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
	public GameObject barricade;

	//TODO Figure out how we can block access to this door when it is locked
	public bool locked = true;
	public bool placed = false;

	void Update(){
		if (locked && !placed) {
			//Please do something to block the door off here
			//Create a barrier to place over the door
			BarricadeDoor();
			placed = true;
		} else if(!locked) {
			//Please remove whatever it is that you did in order to lock the door
			RemoveBarricade();
			placed = false; //free up this update method to allow it to place the barrier again
		}
	}

	//TODO Move the camera to be either only the room, or have the room and the hallway that the people are currently going into
	void OnTriggerEnter2D(Collider2D other){
		//TODO Spawn another room into the map that we have
		//CameraController.S.TransitionCamera(dir, false);
		print("Door Entered");
		CameraController.S.RoomViews(gameObject.transform.parent.transform.position, dir);
	}

	void OnTriggerExit(Collider other){
		//TODO set current game state to be either a room or a hallway
		//This will be used by both DungeonLayout and for camera transitions
	}

	/*
	 * Need to write some functions to handle player movement when the door is called
	 */

	//Prevents players from moving through a locked door
	void BarricadeDoor(){

	}

	//Clears the players' path to allow them access to hallways
	void RemoveBarricade(){

	}
}
