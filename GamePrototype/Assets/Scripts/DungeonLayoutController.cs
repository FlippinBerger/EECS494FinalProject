using UnityEngine;
using System.Collections;

public class DungeonLayoutController : MonoBehaviour {

	//array of layouts to randomly choose from when starting or changing levels
	public DungeonLayout[] layouts;

	//arrays of rooms to randomly populate the current level with
	public Room[] rooms;
	public BossRoom[] bossRooms;

	//offset from the room position that the room interior needs to be instantiated with
	public int offset = 0;

	//Current DungeonLayout
	private DungeonLayout DL;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//func that chooses a dungeon layout and populates it with rooms
	void CreateLevel(){
		DL = layouts[Random.Range(0, layouts.Length)];
		PlaceRoomsWithinLayout ();
	}

	void PlaceRoomsWithinLayout(){
		foreach (Vector3 pos in DL.roomPositions) {
			Room room = rooms [Random.Range (0, rooms.Length)];
			room.transform.position = new Vector3 (pos.x, pos.y, 0);
			Instantiate (room);
		}
		Room bossRoom = bossRooms [Random.Range (0, bossRooms.Length)];
		bossRoom.transform.position = new Vector3 (DL.bossRoomPosition.x + offset, DL.bossRoomPosition.y + offset, 0);
		Instantiate (bossRoom);
	}


}
