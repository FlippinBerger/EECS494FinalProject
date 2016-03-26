using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonLayoutController : MonoBehaviour {

	//array of layouts to randomly choose from when starting or changing levels
	public GameObject[] layouts;

	//arrays of rooms to randomly populate the current level with
	public GameObject[] rooms;
	public GameObject[] bossRooms;

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
		DL = layouts[Random.Range(0, layouts.Length - 1)];
		Instantiate (DL);
		//CreateMinimap(); //Make the minimap game object here based on the DL.matrix
		PlaceRoomsWithinLayout ();
		print("Current room index");
		print (startRoomIndex);
		print (DL.GetComponent<DungeonLayout> ().roomPositions.Length);
		print (DL.GetComponent<DungeonLayout> ().roomPositions [startRoomIndex]);
		CameraController.S.SetCameraPosition(DL.GetComponent<DungeonLayout>().roomPositions[startRoomIndex]);
	}

	void PlaceRoomsWithinLayout(){
		string[] roomMatrix = DL.GetComponent<DungeonLayout> ().matrix;
		for (int row = 0; row < roomMatrix.Length - 1; ++row) {
			for (int col = 0; col < roomMatrix[row].Length - 1; ++col) {
				if (roomMatrix [row] [col] == '1') {
					Direction[] doorDirectionsNeeded = DungeonLayoutGenerator.GetDoorDirs(roomMatrix, col, row, roomMatrix.Length);
					GameObject room = GetRoomWithDirections (doorDirectionsNeeded);
					room.transform.position = new Vector3 (col * roomWidth, row * -1 * roomHeight, 0);
					Instantiate (room);
				}
			}
		}
		GetStartRoomIndex();
	}

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
		return roomArray [Random.Range (0, roomArray.Length - 1)];
	}

	//function that randomly chooses a room from the matrix to make the start room
	void GetStartRoomIndex(){
		startRoomIndex = Random.Range (0, DL.GetComponent<DungeonLayout> ().roomIndex);
	}
}


//Have an array of 
