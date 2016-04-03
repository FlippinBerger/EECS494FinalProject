using UnityEngine;
using System.Collections;

//This script gets attached to each layout
//It knows the positions of all the room skeletons so that it can be
//used to place rooms 
public class DungeonLayout : MonoBehaviour {

	public Vector3[] roomPositions;
	public Vector3 startRoomPosition;
	public Vector3 bossRoomPosition;
	public string[] matrix;

	public Element element;

	public int roomIndex = 0;

	public void Init(int numRooms, string[] lines){
		roomPositions = new Vector3[numRooms];
		startRoomPosition = Vector3.zero;
		bossRoomPosition = Vector3.zero;
		matrix = lines;

		element = GameManager.S.GetRandomElement ();
	}

	public void AddRoomPosition(Vector3 pos){
		roomPositions [roomIndex] = pos;
		++roomIndex;
	}

	//Comes from the generator
	//Once this is instantiated, have a "populate yoself method" to put all the rooms where they need to go.
	//also adds in the hallways where they need to go 
	//based on the element needs to Instantiate rooms.
	public void PopulateLayout(){
		int roomHeight = GameManager.S.roomHeight;
		int roomWidth = GameManager.S.roomWidth;

		for (int row = 0; row < matrix.Length; ++row) {
			for (int col = 0; col < matrix [0].Length; ++col) {
				if (isRoom (matrix [row] [col])) {
					Vector3 roomPos = MakeRoom (row, col);
					Direction[] doors = AddDoors (roomPos, row, col);
					AddHallways (doors, roomPos, row, col);
				}
			}
		}
	}

	//instantiate room in the proper position
	private Vector3 MakeRoom(int row, int col){
		TextAsset roomFile = GameManager.S.GetRandomRoomFile ();
		GameObject room = RoomImporter.S.CreateRoom (roomFile, element);
		room.transform.position = MakeRoomPosition (row, col);
		Instantiate (room);
        return room.transform.position;
	}

	private Vector3 MakeRoomPosition(int row, int col){
		Vector3 pos = new Vector3 (col * GameManager.S.roomWidth + col * GameManager.S.hallLength,
			row * GameManager.S.roomWidth + row * GameManager.S.hallLength, 0);
		return pos;
	}

	//Properly places the doors within a room
	//returns the Direction array so that it can be used to determine hallways as well
	private Direction[] AddDoors(Vector3 pos, int row, int col){
		Direction[] doorsNeeded = GetDoorDirs (row, col);

		//flip if door is vertical (Left and Right)
		bool flip = false;
		for (int i = 0; i < doorsNeeded.Length; ++i) {
			flip = false;
			switch (doorsNeeded [i]) {
			case Direction.Up:
				pos = new Vector3 (pos.x + GameManager.S.h_UpAndDown, pos.y + GameManager.S.roomHeight, 0);
				flip = true;
				break;
			case Direction.Down:
				pos = new Vector3 (pos.x + GameManager.S.h_UpAndDown, pos.y, 0);
				flip = true;
				break;
			case Direction.Left:
				pos = new Vector3 (pos.x, pos.y + GameManager.S.v_LeftAndRight, 0);
				break;
			case Direction.Right:
				pos = new Vector3 (pos.x + GameManager.S.roomWidth, pos.y + GameManager.S.v_LeftAndRight, 0);
				break;
			default:
				break;
			}
			GameManager.S.door.transform.position = pos;
			if (flip) {
				GameManager.S.door.transform.Rotate (new Vector3 (0, 0, 90));
			}
			Instantiate (GameManager.S.door);
		}
		return doorsNeeded;
	}

	//Adds a hallway between 2 rooms
	//Only add Right and Down hallways. Left and Up would be backtracking.
	private void AddHallways(Direction[] dirs, Vector3 pos, int row, int col){
		bool flip = false;
		foreach (Direction dir in dirs) {
			flip = false;
			if (dir == Direction.Down) {
				pos = new Vector3 (pos.x + GameManager.S.h_UpAndDown - 1, pos.y - 1, 0);
				flip = true;
			} else if (dir == Direction.Right) {
				pos = new Vector3 (pos.x + GameManager.S.roomWidth + 1, pos.y + GameManager.S.v_LeftAndRight - 1, 0);
			}
			GameManager.S.hallway.transform.position = pos;
			if (flip) {
				GameManager.S.hallway.transform.Rotate (0, 0, -90);
			}
			Instantiate (GameManager.S.hallway);
		}
	}

	//returns true if the character is a room character
	private bool isRoom(char c){
		return c == 'S' || c == 'B' || c == '1';
	}

	//function that checks the surrounding positions in the file for doors
	public static Direction[] GetDoorDirs(int height, int pos){
		Direction[] doorDirs = new Direction[]{Direction.None, Direction.None, Direction.None, Direction.None};

        /*
		if (height > 0) {
			if (isRoom(matrix [height - 1] [pos])) {
				doorDirs [0] = Direction.Up;
			}
		}
		if(height < matrix.Length - 1){
			if (isRoom(matrix [height + 1] [pos])) {
				doorDirs [1] = Direction.Down;
			} 
		}
		if (pos > 0) {
			if (isRoom(matrix [height] [pos - 1])) {
				doorDirs [2] = Direction.Left;
			}
		}
		if(pos < matrix[0].Length - 1){
			if (isRoom(matrix [height] [pos + 1])) {
				doorDirs [3] = Direction.Right;
			}
		}
        */
		return doorDirs;
	}
}
