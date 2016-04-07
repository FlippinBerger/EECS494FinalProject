using UnityEngine;
using System.Collections;

//Used to keep track of which Camera view to use (Room is zoomed in, Hallways is zoomed out)
public enum CameraState {
	Room, Hallway
};

public class CameraController : MonoBehaviour {

	static public CameraController S;

	public bool roomEntered = false;
	public CameraState state;

	public int hallwayTransitionNum = 0;
	public bool fromRoom = false; //used to tell the hallway transition that it has already been set from the room
	public Direction prevDir = Direction.None; //used to keep track of the last transition that happened
	public Direction lastDoorDir = Direction.None;

	//Initial offsets based on room pos
	private float horizontalOffset = 11.5f;
	private float verticalOffset = 7.5f;

	//Transition offsets to be used when players are moving through the dungeon
	private float verticalTrans = 8f;
	private float roomToHall = 5f;
	private float hallToHall = 22f;

	public float cdDuration = 0.0f;
	public float cdStartTime = 0.0f;

	//if(Time.Time - cdStartTime > cdDuration) //cd is finished if this is true

	void Awake(){
		S = this;
	}

	//Places the camera in accordance to the provided start room vector
	public void SetCameraPosition(Vector3 roomPos){
		Camera.main.transform.position = new Vector3 (roomPos.x + horizontalOffset, roomPos.y - .5f, -10f);
		state = CameraState.Hallway;
		roomEntered = false;
	}

	public void PutCameraInHallway (Direction d){
		print ("PutCameraInHall with dir " + d);
		
		if (!roomEntered) { //hacky to fix transition issues in start hall
			return;
		}
		Vector3 currentPos = gameObject.transform.position;
		Vector3 newPos = currentPos;

		if (state == CameraState.Hallway) { //Do a hall camera swap
			Direction dir = lastDoorDir;
			if (hallwayTransitionNum % 2 == 1)  //even means you're going towards the next room still in hall though
				dir = GetOppositeDirection (dir);
			switch (dir) {
				case Direction.Up:
					newPos.y += 8;
					break;
				case Direction.Down:
					newPos.y -= 8;
					break;
				case Direction.Left:
					newPos.x -= 22;
					break;
				case Direction.Right:
					newPos.x += 22;
					break;
			}
			++hallwayTransitionNum;
		}
		state = CameraState.Hallway;
		gameObject.transform.position = newPos;
	}

	public void RoomViews(Direction d){
		if (Time.time - cdStartTime <= cdDuration)
			return;
		if (!roomEntered) { //meta data for first transition into a room
			roomEntered = true;
			cdDuration = 1.5f;
		}
		hallwayTransitionNum = 0;
		print ("Roomview with dir " + d);

		cdStartTime = Time.time;
		Vector3 newPos = gameObject.transform.position;

		if (state == CameraState.Hallway) { //Move cam to be just the room
			state = CameraState.Room;
			newPos = GetCameraPosForRoomView (d);
		} else { //Move cam to be room + hallway view
			fromRoom = true;
			state = CameraState.Hallway;
			newPos = GetCameraPosForRoomView(GetOppositeDirection(d));
		}
		gameObject.transform.position = newPos;
		prevDir = d;
	}

	//Puts camera on the room
	Vector3 GetCameraPosForRoomView(Direction d){
		Vector3 newPos = gameObject.transform.position;
		switch (d) {
		case Direction.Up:
			newPos.y -= 8f;
			break;
		case Direction.Down:
			newPos.y += 8f;
			break;
		case Direction.Left:
			newPos.x += 5;
			break;
		case Direction.Right:
			newPos.x -= 5;
			break;
		}
		return newPos;
	}

	Direction GetOppositeDirection(Direction d){
		switch (d) {
		case Direction.Up:
			return Direction.Down;
		case Direction.Down:
			return Direction.Up;
		case Direction.Left:
			return Direction.Right;
		case Direction.Right:
			return Direction.Left;
		default:
			return Direction.None;	
		}
	}
}
