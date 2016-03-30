using UnityEngine;
using System;
using System.Collections;
using System.IO;

//class that generates level layouts to create into prefabs
public class DungeonLayoutGenerator : MonoBehaviour {

	//name of the file containing the layout that we want to use for our dungeon.
	public TextAsset layoutFile;

    // might not need this public member, can probably just make an empty gameobject to parent everything
	public GameObject dungeonLayoutPrefab; //Prefab used to create all Layouts
	private GameObject parentLayout; //Actual layout to be worked with and saved at the end

	//room dimensions to nicely fit a 16x9 aspect ratio
	public int roomWidth = 24;
	public int roomHeight = 16;

    // TODO instead of relying on Start, make a static "create level" function which GameManager calls
    // the functino will also pick a random element, text file, etc.

	// Use this for initialization
	void Start () {
		parentLayout = Instantiate (dungeonLayoutPrefab);
		parentLayout.transform.position = new Vector3 (0, 0, 0);
		LoadMapFile (layoutFile); // 1s and 0s
	}

	string CleanLine(string line) {
		line = line.Trim('\r'); // trim windows newline
		return line;
	}

	//function that actually creates the entire level layout
	private void LoadMapFile(TextAsset file){
		try {
			string[] lines = file.text.Trim().Split('\n'); //split the file into lines

			int numRooms = GetNumRooms(lines); //number of rooms to init a DungeonLayout
			DungeonLayout DL = parentLayout.AddComponent<DungeonLayout>();
			DL.Init(numRooms, lines);

			int height = lines.Length - 1;

			string line;
			for(int y = 0; y < height; ++y){
				line = lines[y];
				int width = line.Length - 1;
				line = CleanLine(line);

				if(line != null){
					for(int x = 0; x < width; ++x){ //for each char in the line
						if(line[x] == '1'){
							Vector3 roomPos = new Vector3(x * roomWidth, y * roomHeight, 0);
							DL.AddRoomPosition(roomPos);
						}
					}
				}
			}
		} catch(Exception e){
			Console.WriteLine("{0}\n", e.Message);
			print (e.Message);
		}
	}

	//counts the number of rooms in the file
	private int GetNumRooms(string[] lines){
		int count = 0;
		for (int i = 0; i < lines.Length; ++i) {
			int cols = lines [i].Length;
			for (int j = 0; j < cols; ++j) {
				if (lines [i] [j] == '1') {
					++count;
				}
			}
		}
		return count;
	}

	//function that checks the surrounding positions in the file for doors
	public static Direction[] GetDoorDirs(string[] lines, int pos, int height, int maxHeight){
		Direction[] doorDirs = new Direction[]{Direction.None, Direction.None, Direction.None, Direction.None};
		if (height > 0) {
			if (lines [height - 1] [pos] == '1') {
				doorDirs [0] = Direction.Up;
			}
		}
		if(height < maxHeight - 2){
			if (lines [height + 1] [pos] == '1') {
				doorDirs [1] = Direction.Down;
			} 
		}
		if (pos > 0) {
			if (lines [height] [pos - 1] == '1') {
				doorDirs [2] = Direction.Left;
			}
		}
		if(pos < lines.Length - 1){
			if (lines [height] [pos + 1] == '1') {
				doorDirs [3] = Direction.Right;
			}
		}
		return doorDirs;
	}
}


/*
 * So lets say a room starts at 0 0
 * a door going west should be at 0 8 and 0 9
 * a door going east should be at 32 8 and 32 9 
 * a door going south should be at 15 0 and 16 0
 * a door going north should be at 15 18 and 16 18
 */
