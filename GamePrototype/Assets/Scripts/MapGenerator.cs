using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;

public class MapGenerator : MonoBehaviour {

	static public MapGenerator S;

	//name of the room file
	public string fileName;

	//contents of the room file in string format
	private string fileContents;
	private int roomHeight;
	private int roomWidth;

	private Map currMap;

	public GameObject floorTile1;

	void Start(){
		ReadFile ();
		ParseRoomSize ();
		currMap = MakeMapFromFile ();
	}


	private void ReadFile(){
		string[] lines = System.IO.File.ReadAllLines ("Assets/Rooms/" + fileName + ".txt");
        fileContents = string.Join("\n", lines);
	}

	private void ParseRoomSize(){
		bool flag = true;

		string[] contentParts = fileContents.Split (',');
		if (contentParts.Length != 3) {
			Console.WriteLine ("Room parsed incorrectly and has " + contentParts.Length.ToString() + " components.");
		}

		flag = int.TryParse (contentParts [0], out roomHeight);
		CheckFlag (flag, "roomHeight");
		flag = int.TryParse (contentParts [1], out roomWidth);
		CheckFlag (flag, "roomWidth");

		//set fileContents to the remaining room content data
		fileContents = contentParts[2];
	}

	//prints an error message if the flag wasn't read properly
	private void CheckFlag(bool flag, string s){
		if (!flag) {
			print("flag not read in correctly for " + s);
		}
	}

	//Creates and returns the Map of the current room
	public Map MakeMapFromFile(){
		Map map = new Map (roomHeight + 1, roomWidth);

		string[] roomRows = fileContents.Split ('\n');

		for (int i = 1; i <= roomHeight; ++i) {
			for (int j = 0; j < roomWidth; j++) {
				print (i.ToString () + " " + j.ToString ());
				if (roomRows [i] [j] == '-') {
					map.map[i, j] = new Tile (TileType.Floor, i, j);
					floorTile1.transform.position = map.map [i, j].pos;
					Instantiate (floorTile1);
				}
			}
		}
		return map;
	}
		
}
