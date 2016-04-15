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

	/*
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

	//We set 

	//TODO Move the camera to be either only the room, or have the room and the hallway that the people are currently going into
	void OnTriggerEnter2D(Collider2D other){
		//TODO Spawn another room into the map that we have
		//CameraController.S.TransitionCamera(dir, false);
		if (other.CompareTag ("Player")) {
			CameraController.S.lastDoorDir = dir;
			print ("Door Entered");
			CameraController.S.RoomViews (dir);

			Room room = GetComponentInParent<Room> ();
			room.currentRoom = true;
			if (CameraController.S.roomSet)
				GameManager.S.currentRoom.currentRoom = false;
			else
				CameraController.S.roomSet = true;
			GameManager.S.currentRoom = room;
		}
	}

*/
}
