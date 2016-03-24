using UnityEngine;
using System.Collections;

//This script gets attached to each layout
//It knows the positions of all the room skeletons so that it can be
//used to place rooms 
public class DungeonLayout : MonoBehaviour {

	static public int maxSize = 5;

	public Vector3[] roomPositions;
	public Vector3 bossRoomPosition;
	public string[] matrix;

	private int roomIndex;

	public DungeonLayout(int numRooms){
		roomPositions = new Vector3 [numRooms];
		bossRoomPosition = new Vector3 (0, 0, 0);
	}

	public void Init(int numRooms, string[] lines){
		roomPositions = new Vector3[numRooms];
		bossRoomPosition = new Vector3(0, 0, 0);
		matrix = lines;
	}

	public void AddRoomPosition(Vector3 pos){
		roomPositions [roomIndex++] = pos;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
