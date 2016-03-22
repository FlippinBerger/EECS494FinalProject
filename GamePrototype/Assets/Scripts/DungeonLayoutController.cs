using UnityEngine;
using System.Collections;

public class DungeonLayoutController : MonoBehaviour {

	//array of layouts to randomly choose from when starting or changing levels
	public GameObject[] layouts;

	//arrays of rooms to randomly populate the current level with
	public GameObject[] rooms;
	public GameObject[] bossRooms;

	//offset from the room position that the room interior needs to be instantiated with
	public int offset = 0;

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
		PlaceRoomsWithinLayout ();
	}

	void PlaceRoomsWithinLayout(){
		print ("In PlaceRooms");
		print (DL.GetComponent<DungeonLayout> ().roomPositions.Length);
		foreach (Vector3 pos in DL.GetComponent<DungeonLayout>().roomPositions) {
			print ("creating a room");
			GameObject room = rooms [Random.Range (0, rooms.Length - 1)];
			room.transform.position = new Vector3 (pos.x, pos.y, 0);
			Instantiate (room);
		}
		print("rooms have been placed");
		if (bossRooms.Length == 0)
			return;
		GameObject bossRoom = bossRooms [Random.Range (0, bossRooms.Length - 1)];
		bossRoom.transform.position = new Vector3 (DL.GetComponent<DungeonLayout>().bossRoomPosition.x + offset, DL.GetComponent<DungeonLayout>().bossRoomPosition.y + offset, 0);
		Instantiate (bossRoom);
	}


}
