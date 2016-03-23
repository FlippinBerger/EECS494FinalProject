using UnityEngine;
using System;
using System.Collections;
using System.IO;

//class that generates level layouts to create into prefabs
public class DungeonLayoutGenerator : MonoBehaviour {

	//name of the file containing the layout that we want to use for our dungeon.
	public TextAsset layoutFile;

	public GameObject dungeonLayoutPrefab; //Prefab used to create all Layouts
	private GameObject parentLayout; //Actual layout to be worked with and saved at the end

	//room dimensions to nicely fit a 16x9 aspect ratio
	public int roomWidth = 24;
	public int roomHeight = 16;

	/*
	//vertical offset for Left and Right doors 
	public int LRVerticalOffset1 = 7;
	public int LRVerticalOffset2 = 8;

	//horizontal offset for Up and Down doors
	public int UDHorizontalOffset1 = 11;
	public int UDHorizontalOffset2 = 12;
	*/

	// Use this for initialization
	void Start () {
		parentLayout = Instantiate (dungeonLayoutPrefab);
		parentLayout.transform.position = new Vector3 (0, 0, 0);
		LoadMapFile (layoutFile);
	}

	string CleanLine(string line) {
		line = line.Trim('\r'); // trim windows newline
		return line;
	}

	//function that actually creates the entire level layout
	private void LoadMapFile(TextAsset file){
		try {
			print("Loading Layout");
			string[] lines = file.text.Trim().Split('\n'); //split the file into lines

			int numRooms = GetNumRooms(lines); //number of rooms to init a DungeonLayout
			DungeonLayout DL = parentLayout.AddComponent<DungeonLayout>();
			DL.Init(numRooms);

			int height = lines.Length;

			string line;
			for(int y = 0; y < height; ++y){
				line = lines[y];
				int width = line.Length;
				line = CleanLine(line);

				print("Line " + y.ToString() + ": \"" + line + "\"");
				if(line != null){
					for(int x = 0; x < width; ++x){ //for each char in the line
						if(line[x] != '1'){ //current position is not a room so continue loop
							continue;
						}
						Direction[] doorDirs = GetDoorDirs(lines, x, y, lines.Length);
						Vector3 roomPos = new Vector3(0, 0);
						for(int i = 0; i < doorDirs.Length; ++i){

							//Get the positions in which to place doors
							Vector3 pos1 = new Vector3(0,0);
							Vector3 pos2 = new Vector3(0,0);
							bool flag = true;
							switch(doorDirs[i]){
								case Direction.Up:
									pos1 = new Vector3(x * roomWidth, y * roomHeight + roomHeight - 1, 0);
									pos2 = new Vector3(x * roomWidth, y * roomHeight + roomHeight - 1, 0);
									break;
								case Direction.Down:
									pos1 = new Vector3(x * roomWidth, y * roomHeight, 0);
									pos2 = new Vector3(x * roomWidth, y * roomHeight, 0);
									break;
								case Direction.Left:
									pos1 = new Vector3(x * roomWidth, y * roomHeight, 0);
									pos2 = new Vector3(x * roomWidth, y * roomHeight, 0);
									break;
								case Direction.Right:
									pos1 = new Vector3(x * roomWidth + roomWidth - 1, y * roomHeight, 0);
									pos2 = new Vector3(x * roomWidth + roomWidth - 1, y * roomHeight, 0);
									break;
								default:
									flag = false;
									break;
							}
							/*
							if(flag){
								PlaceDoor(pos1, pos2);
							} else {
								flag = true;
							}
							*/
						}
						roomPos = new Vector3(x * roomWidth, y * roomHeight, 0);
						DL.AddRoomPosition(roomPos);
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
	private Direction[] GetDoorDirs(string[] lines, int pos, int height, int maxHeight){
		Direction[] doorDirs = new Direction[]{Direction.None, Direction.None, Direction.None, Direction.None};
		print ("getting door directions");
		if (height > 0) {
			if (lines [height - 1] [pos] == '1') {
				print ("adding up door");
				doorDirs [0] = Direction.Up;
			}
		}
		if(height < maxHeight - 2){
			if (lines [height + 1] [pos] == '1') {
				print ("adding down door");
				doorDirs [1] = Direction.Down;
			} 
		}
		if (pos > 0) {
			if (lines [height] [pos - 1] == '1') {
				print ("adding left door");
				doorDirs [2] = Direction.Left;
			}
		}
		if(pos < lines.Length - 1){
			if (lines [height] [pos + 1] == '1') {
				print ("adding right door");
				doorDirs [3] = Direction.Right;
			}
		}
		foreach (Direction dir in doorDirs) {
			print (dir);
		}
		return doorDirs;
	}

	/*
	private void PlaceDoor(Vector3 pos1, Vector3 pos2){
		print ("Adding door at: ");
		print (pos1);
		print (pos2);
		GameObject door1 = Instantiate (doorTile);
		door1.transform.position = pos1;
		door1.transform.parent = parentLayout.transform;

		GameObject door2 = Instantiate (doorTile);
		door2.transform.position = pos2;
		door2.transform.parent = parentLayout.transform;
	}
	*/
}


/*
 * So lets say a room starts at 0 0
 * a door going west should be at 0 8 and 0 9
 * a door going east should be at 32 8 and 32 9 
 * a door going south should be at 15 0 and 16 0
 * a door going north should be at 15 18 and 16 18
 */
