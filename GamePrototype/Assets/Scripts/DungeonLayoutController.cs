using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonLayoutController : MonoBehaviour {

	//array of layouts to randomly choose from when starting or changing levels
	public GameObject[] layouts;

	//arrays of rooms to randomly populate the current level with
	public GameObject[] rooms;
	public GameObject[] bossRooms;

	public GameObject wallPrefab;

	public int roomWidth = 24;
	public int roomHeight = 16;

	public int startRoomIndex;

	//Current DungeonLayout
	private GameObject DL;

	// Use this for initialization
	void Start () {
		CreateLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//func that chooses a dungeon layout and populates it with rooms
	void CreateLevel(){
		DL = layouts[Random.Range(0, layouts.Length)];
		Instantiate (DL);
		//CreateMinimap(); //Make the minimap game object here based on the DL.matrix
		PlaceRoomsWithinLayout ();
		print("Current room index");
		print (startRoomIndex);
		print (DL.GetComponent<DungeonLayout> ().roomPositions.Length);
		print (DL.GetComponent<DungeonLayout> ().roomPositions [startRoomIndex]);
		CameraController.S.SetCameraPosition(DL.GetComponent<DungeonLayout>().roomPositions[startRoomIndex]);
	}

	//Reads through the matrix and places rooms where they should be based on the layout
	void PlaceRoomsWithinLayout(){
		string[] roomMatrix = DL.GetComponent<DungeonLayout> ().matrix;
		for (int row = 0; row < roomMatrix.Length - 1; ++row) {
			for (int col = 0; col < roomMatrix[row].Length - 1; ++col) {
				if (roomMatrix [row] [col] == '1') {
					Direction[] doorDirectionsNeeded = DungeonLayoutGenerator.GetDoorDirs(roomMatrix, col, row, roomMatrix.Length);
					GameObject room = GetRoomWithDirections (doorDirectionsNeeded);
					room.transform.position = new Vector3 (col * roomWidth, row * roomHeight, 0);
					if (room.transform.position.y > 0) {
						room.transform.position = new Vector3 (room.transform.position.x, room.transform.position.y * -1, 0);
					}
					Instantiate (room);
					//RemoveUnusedDoors (row, col, room.GetComponent<Room>().doors, doorDirectionsNeeded);
				}
			}
		}
		GetStartRoomIndex();
	}

	/*
	//Puts wall tiles over doors that shouldn't be there according to the layout
	//if there are dirs in doorsInRoom that aren't in doorsNeeded, cover them with walls
	void RemoveUnusedDoors(int row, int col, Direction[] doorsInRoom, Direction[] doorsNeeded){
		Direction[] dirs = GetDoorDifferences (doorsInRoom, doorsNeeded);
		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;

		foreach (Direction dir in dirs) {
			switch (dir) {
			case Direction.Up:
				FindDoorPositions ();
				break;
			case Direction.Down:
				break;
			case Direction.Left:
				break;
			case Direction.Right:
				break;
			default:
				break;
			}
		}
	}


	//returns an array of all the Directions that need to be wrote over
	Direction[] GetDoorDifferences(Direction[] doorsInRoom, Direction[] doorsNeeded){
		List<Direction> dirs = new List<Direction> ();
		bool flag = false;
		foreach (Direction d in doorsInRoom) {
			foreach (Direction dn in doorsNeeded) {
				if (d == dn) {
					flag = true;
					break;
				}
			}
			if (!flag) {
				dirs.Add (d);
				flag = true;
			}
		}
		return dirs.ToArray ();
	}
	*/

	//Currently grabs any room in the array that has all the dirs available (can block off unnecessary doors with wallTiles)
	GameObject GetRoomWithDirections(Direction[] dirs){
		List<GameObject> usableRooms = new List<GameObject>();
		foreach (GameObject room in rooms) {
			if (room.GetComponent<Room> ().HasDoors (dirs)) {
				usableRooms.Add (room);
			}
		}
		GameObject[] roomArray = usableRooms.ToArray ();
		print ("room array size: ");
		print (roomArray.Length);
		return roomArray [Random.Range (0, roomArray.Length)];
	}

	//function that randomly chooses a room from the matrix to make the start room
	void GetStartRoomIndex(){
		startRoomIndex = Random.Range (0, DL.GetComponent<DungeonLayout> ().roomIndex);
	}
}


//Have an array of 
