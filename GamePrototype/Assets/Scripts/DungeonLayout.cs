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

	public int roomIndex = 0;

	public void Init(int numRooms, string[] lines){
		roomPositions = new Vector3[numRooms];
		startRoomPosition = Vector3.zero;
		bossRoomPosition = Vector3.zero;
		matrix = lines;
	}

	public void AddRoomPosition(Vector3 pos){
		roomPositions [roomIndex] = pos;
		++roomIndex;
	}
		
	//also adds in the hallways where they need to go 
	//based on the element needs to Instantiate rooms.
	public void PopulateLayout()
    {
        print(matrix.Length);
        print(matrix[0].Length);
        for (int row = 0; row < matrix.Length; ++row) {
			for (int col = 0; col < matrix [0].Length; ++col) {
				if (isRoom (matrix [row] [col])) {
					Vector3 roomPos = MakeRoom (row, col);
					print ("Room position: " + roomPos);
					Direction[] doors = AddDoors (roomPos, row, col);
					AddHallways (doors, roomPos, row, col);
				}
			}
		}
	}

	//instantiates room in the proper position
	Vector3 MakeRoom(int row, int col){
		TextAsset roomFile = GameManager.S.GetRandomRoomFile ();
		GameObject room = RoomImporter.S.CreateRoom (roomFile, GameManager.S.currentLevelElement);
		room.transform.position = MakeRoomPosition (row, col);
		return room.transform.position; //used to place hallways 
	}

	//returns a v3 based on the current layout position and the need of hallways
	Vector3 MakeRoomPosition(int row, int col){
		Vector3 pos = new Vector3 (col * GameManager.S.roomWidth + col * GameManager.S.hallLength,
			(row * GameManager.S.roomHeight + row * GameManager.S.hallLength) * -1, 0);
		return pos;
	}

	//Properly places the doors and walls off unused doors within a room
	//returns the Direction array so that it can be used to determine hallways as well
	Direction[] AddDoors(Vector3 pos, int row, int col){
		Direction[] doorsNeeded = GetDoorDirs (row, col);
		Vector3 objPos = Vector3.zero;
		bool flip = false; //used to determine if the object should be vertical or horizontal
		Direction objDir = Direction.None;
		for (int i = 0; i < doorsNeeded.Length; ++i) {
			switch (i) {
			case 0: //Up
				objPos = new Vector3 (pos.x + GameManager.S.h_UpAndDown, pos.y + GameManager.S.roomHeight - 1, 0);
				objDir = doorsNeeded [i];
				break;
			case 1: //Down
				objPos = new Vector3 (pos.x + GameManager.S.h_UpAndDown, pos.y, 0);
				objDir = doorsNeeded[i];
				break;
			case 2: //Left
				objPos = new Vector3 (pos.x, pos.y + GameManager.S.v_LeftAndRight, 0);
				objDir = doorsNeeded [i];
				flip = true;
				break;
			case 3: //Right
				objPos = new Vector3 (pos.x + GameManager.S.roomWidth - 1, pos.y + GameManager.S.v_LeftAndRight, 0);
				objDir = doorsNeeded [i];
				flip = true;
				break;
			}
			if (objDir == Direction.None) { //Place a wall fixture here
				GameObject wall = Instantiate(GameManager.S.wallFixture);
				wall.transform.position = objPos;
				if (flip) {
					wall.transform.Rotate(new Vector3(0, 0, -90));
				}
			} else { //Place the door in the correct location
				GameObject door = Instantiate(GameManager.S.door);
				door.transform.position = objPos;
				door.GetComponent<Door> ().dir = objDir;
				if (flip) {
					door.transform.Rotate (new Vector3 (0, 0, -90));
				}
			}
		}
		return doorsNeeded;
	}

	//Adds a hallway between 2 rooms
	//Only add Right and Down hallways. Left and Up would be backtracking.
	//TODO: 
	void AddHallways(Direction[] dirs, Vector3 pos, int row, int col){
		foreach (Direction dir in dirs) {
			if (dir == Direction.Down || dir == Direction.Right) {
				CreateHallway (dir, pos);
			}
		}
	}

	//Gives the position of the parent hallway GO based on the room you're appending it to
	Vector3 GetHallwayPosition(Direction dir, Vector3 roomPos){
		Vector3 hallPos = Vector3.zero;
		switch (dir) {
		case Direction.Down:
			hallPos = new Vector3 (roomPos.x + GameManager.S.h_UpAndDown - 4, roomPos.y - 8, 0);
			break;
		case Direction.Right:
			hallPos = new Vector3 (roomPos.x + GameManager.S.roomWidth, roomPos.y + GameManager.S.v_LeftAndRight + 4, 0);
			break;
		}
		return hallPos;
	}


	//Creates the hallway GO using the proper elemental floor tiles
	//Dir is the direction the hallway is in relation to the room you're appending it to
	//roomPosition is the pos of the room you're appending to
	GameObject CreateHallway(Direction dir, Vector3 roomPosition){
		GameObject hallway = new GameObject ("Hallway");
		hallway.transform.position = GetHallwayPosition (dir, roomPosition);
		// Vector3 pos = hallway.transform.position;
		int rows = GameManager.S.hallWidth;
		int cols = GameManager.S.hallLength;
		for (int i = 0; i < rows; ++i) {
			for (int j = 0; j < cols; ++j) {
				if ((i == 0) || (i == rows - 1)) {
					GameObject wall = Instantiate (GameManager.S.wallTile);
					wall.transform.parent = hallway.transform;
					wall.transform.localPosition = new Vector3 (i, j, 0);
				} else {
					GameObject floor = Instantiate(GameManager.S.floorTile);
					floor.GetComponent<SpriteRenderer> ().sprite = GameManager.S.floorTileSprites [(int)GameManager.S.currentLevelElement];
					floor.transform.parent = hallway.transform;
					floor.transform.localPosition = new Vector3 (i, j, 0);
				}
			}
		}
		if (dir == Direction.Right)
			hallway.transform.Rotate (new Vector3 (0, 0, -90));
		return hallway;
	}

	//returns true if the character is a room character
	bool isRoom(char c){
		return (c == 'S') || (c == 'B') || (c == '1');
	}

	//function that checks the surrounding positions in the file for doors
	public Direction[] GetDoorDirs(int height, int pos){
		Direction[] doorDirs = new Direction[]{Direction.None, Direction.None, Direction.None, Direction.None};
		if (height > 0) {
			if (isRoom(matrix[height - 1][pos])) {
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
		return doorDirs;
	}
}
