using UnityEngine;
using System.Collections;

//Used to keep track of which Camera view to use (Room is zoomed in, Hallways is zoomed out)
public enum CameraState {
	Room, Hallway
};

public class CameraController : MonoBehaviour {

	static public CameraController S;

	public CameraState state;

	public int numTimesDirUsed = 0;
	public Direction prevDir = Direction.None; //used to keep track of the last transition that happened

	//Initial offsets based on room pos
	private float horizontalOffset = 11.5f;
	private float verticalOffset = 7.5f;

	//Transition offsets to be used when players are moving through the dungeon
	private float verticalTrans = 8f;
	private float roomToHall = 5f;
	private float hallToHall = 22f;

	public float cdDuration = 5f;
	public float cdStartTime = 0.0f;

	//if(Time.Time - cdStartTime > cdDuration) //cd is finished if this is true

	void Awake(){
		S = this;
	}

	//Places the camera in accordance to the provided room vector
	public void SetCameraPosition(Vector3 roomPos){
		Camera.main.transform.position = new Vector3 (roomPos.x + horizontalOffset, roomPos.y - .5f, -10f);
		state = CameraState.Hallway;
	}

	public void PutCameraInHallway (Direction d, Vector3 hallParent, bool hallOffset){
		if (Time.time - cdStartTime <= cdDuration)
			return;
		cdStartTime = Time.time;

		state = CameraState.Hallway;
		Vector3 currentPos = gameObject.transform.position;
		Vector3 newPos = Vector3.zero;

		if (d == Direction.Down) { //offset will give the higher camera view
			if (hallOffset) {
				hallParent = new Vector3 (hallParent.x, hallParent.y + 4, hallParent.z);
				newPos = new Vector3 (hallParent.x + 2.5f, hallParent.y - 3.5f, -10f);
			} else {
				newPos = new Vector3 (hallParent.x + 2.5f, hallParent.y - 0.5f, -10f);
			}
		} else if (d == Direction.Right) { //offset gives the further right camera view
			if (hallOffset) {
				hallParent = new Vector3 (hallParent.x + 4, hallParent.y, hallParent.z);
				newPos = new Vector3 (hallParent.x - 7.5f, hallParent.y - 2.5f, -10f);
			} else {
				newPos = new Vector3 (hallParent.x + 10.5f, hallParent.y - 2.5f, -10f);
			}
		}
		gameObject.transform.position = newPos;
	}

	public void RoomViews(Vector3 roomPos, Direction d){
		if (Time.time - cdStartTime <= cdDuration)
			return;
		cdStartTime = Time.time;
		Vector3 newPos = Vector3.zero;

		if (state == CameraState.Hallway) { //Move cam to be just the room
			state = CameraState.Room;
			newPos = GetCameraPosForRoomView (roomPos, GetOppositeDirection(d));
		} else { //Move cam to include the hallway as well
			state = CameraState.Hallway;
			newPos = GetCameraPosForHallwayView (roomPos, d);
		}

		gameObject.transform.position = newPos;
		prevDir = d;
	}

	//Puts camera on the room
	Vector3 GetCameraPosForRoomView(Vector3 roomPos, Direction d){
		Vector3 currentPos = gameObject.transform.position;
		Vector3 newPos = new Vector3 (roomPos.x + 11.5f, roomPos.y + 7.5f, -10);
		return newPos;
	}

	Vector3 GetCameraPosForHallwayView(Vector3 roomPos, Direction d){
		Vector3 currentPos = gameObject.transform.position;
		Vector3 newPos = Vector3.zero;
		Direction used = d;
		if (d == prevDir) {
			used = GetOppositeDirection (d);
		}
		switch (d) {
		case Direction.Up:
			newPos = new Vector3 (roomPos.x, roomPos.y + 16f, -10f);
			break;
		case Direction.Down:
			newPos = new Vector3 (roomPos.x + 11.5f, roomPos.y, -10);
			break;
		case Direction.Left:
			break;
		case Direction.Right:
			break;
		}

		return newPos;
	}

	//All purpose Camera Transition function
	//determines which sub functions to call in order to get the new camera position
	//d will be the direction of the last door you entered
	//it will be Direction.None if you are in a hallway in order to use the last Door position to determine where to go
	public void TransitionCamera(Direction d, bool hallTrans){
		Vector3 currentPos = gameObject.transform.position;
		Vector3 newCameraPos = Vector3.zero;

		if (hallTrans) {
			SwitchHallwayView (d, currentPos);
		} else {
			RoomAndHallwayTransition (d, currentPos);

			if (prevDir == d || prevDir == Direction.None) { //Second part is for hallway transitions
				++numTimesDirUsed;
			} else {
				prevDir = d; //set the lastDir to be what was just used
				numTimesDirUsed = 1;
			}
		}
		gameObject.transform.position = newCameraPos;
	}
		
	//Enters the hallway camera view from a room camera view
	Vector3 RoomAndHallwayTransition(Direction d, Vector3 pos){
		if(d == prevDir)
			d = HandleSameDir (d);
		
		Vector3 newCameraPos = Vector3.zero;
		switch (d) {
		case Direction.Up:
			newCameraPos = new Vector3 (pos.x, pos.y + verticalTrans, pos.z);
			break;
		case Direction.Down:
			newCameraPos = new Vector3 (pos.x, pos.y - verticalTrans, pos.z);
			break;
		case Direction.Left:
			newCameraPos = new Vector3 (pos.x - roomToHall, pos.y, pos.z);
			break;
		case Direction.Right:
			newCameraPos = new Vector3 (pos.x + roomToHall, pos.y, pos.z);
			break;
		}
		state = CameraState.Hallway;
		return newCameraPos;
	}

	//The intermediate switch in camera view between rooms
	Vector3 SwitchHallwayView(Direction d, Vector3 pos){
		Vector3 newCameraPos = Vector3.zero;
		switch (d) {
		case Direction.Up:
			newCameraPos = new Vector3 (pos.x, pos.y + verticalTrans, pos.z);
			break;
		case Direction.Down:
			newCameraPos = new Vector3 (pos.x, pos.y - verticalTrans, pos.z);
			break;
		case Direction.Left:
			newCameraPos = new Vector3 (pos.x - hallToHall, pos.y, pos.z);
			break;
		case Direction.Right:
			newCameraPos = new Vector3 (pos.x - hallToHall, pos.y, pos.z);
			break;
		}
		return newCameraPos;
	}

	//Function used to determine the direction of the transition if the same threshold is entered
	//more than once in a row
	Direction HandleSameDir(Direction d){
		if(numTimesDirUsed % 2 != 0)
			d = GetOppositeDirection (d);
		return d;
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
