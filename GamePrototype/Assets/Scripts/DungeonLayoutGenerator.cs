using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//class that generates level layouts to create into prefabs
public class DungeonLayoutGenerator : MonoBehaviour {

	public static DungeonLayoutGenerator S;

	//name of the file containing the current layout that we want to use for our dungeon level
	public TextAsset layoutFile;

	public GameObject levelLayout; //Actual layout to be worked with

	void Awake(){
		S = this;
	}

    // TODO instead of relying on Start, make a static "create level" function which GameManager calls
    // the functino will also pick a random element, text file, etc.
	public void CreateLevelMap(){
		levelLayout = new GameObject ("levelPrefab"); //Create the game object
		GameManager.S.AddObject(levelLayout);
		levelLayout.transform.position = Vector3.zero; 
		layoutFile = PickRandomLayoutFile ();
		LoadMapFile (layoutFile); //Load the chosen layout file

		//We now have the DungeonLayout stored within levelLayout
		levelLayout.GetComponent<DungeonLayout>().PopulateLayout(); //Place all the rooms within the current level
	}

	//chooses a random TextAsset from the layout assets specified in GameManager and the Inspector
	//Finds the files in the FS and chooses one at random from those available to return
	TextAsset PickRandomLayoutFile(){
		TextAsset ta;
		if (GameManager.S.round <= 2) {
			ta = GameManager.S.easyLayouts [UnityEngine.Random.Range (0, GameManager.S.easyLayouts.Length)];
			//print (ta.name);
			return ta;
		} else if (GameManager.S.round <= 4) {
			ta = GameManager.S.medLayouts [UnityEngine.Random.Range (0, GameManager.S.medLayouts.Length)];
			//print (ta.name);
			return ta;
		} else {
			ta = GameManager.S.hardLayouts[UnityEngine.Random.Range (0, GameManager.S.hardLayouts.Length)];	
			//print (ta.name);
			return ta;
		}
	}

	string CleanLine(string line) {
		line = line.Trim('\r'); // trim windows newline
		return line;
	}

	//function that actually creates the entire level layout
	private void LoadMapFile(TextAsset file){
		try {
			string[] lines = file.text.Trim().Split('\n'); //split the file into lines
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = CleanLine(lines[i]);
            }

			int numRooms = GetNumRooms(lines); //number of rooms to init a DungeonLayout
			DungeonLayout DL = levelLayout.AddComponent<DungeonLayout>();
			DL.Init(numRooms, lines);

			int height = lines.Length - 1;

			string line;
			for(int y = 0; y < height; ++y){
				line = lines[y];
				int width = line.Length - 1;
				line = CleanLine(line);

				if(line != null){
					for(int x = 0; x < width; ++x){ //for each char in the line
						if(line[x] == '1' || line[x] == 'S' || line[x] == 'B'){ //Add all the room positions to the DL
							Vector3 roomPos = MakeRoomPosition(y, x); //row, col
							if(line[x] == 'S'){
								DL.startRoomPosition = roomPos;
							} else if(line[x] == 'B'){
								DL.bossRoomPosition = roomPos;
							}
						}
					}
				}
			}
		} catch(Exception e){
			Console.WriteLine("{0}\n", e.Message);
			print (e.Message);
		}
	}

    internal static Direction[] GetDoorDirs(string[] roomMatrix, int col, int row, int length)
    {
        throw new NotImplementedException();
    }

    //Creates the room position; taking into account both number of rooms and number of hallways
    private Vector3 MakeRoomPosition(int row, int col){
		Vector3 pos = new Vector3 (col * GameManager.S.roomWidth + col * GameManager.S.hallLength,
			(row * GameManager.S.roomHeight + row * GameManager.S.hallLength) * -1, 0);
		return pos;
	}

	//counts the number of rooms in the file
	private int GetNumRooms(string[] lines){
		int count = 0;
		for (int i = 0; i < lines.Length; ++i) {
			int cols = lines [i].Length;
			for (int j = 0; j < cols; ++j) {
				if (lines [i] [j] == '1' || lines[i][j] == 'S' || lines[i][j] == 'B') {
					++count;
				}
			}
		}
		return count;
	}
}


/*
 * So lets say a room starts at 0 0
 * a door going west should be at 0 7, 0 8, 0 9, 0 10
 * a door going east should be at 32 7, 32 8, 32 9, 32 10
 * a door going south should be at 14 0, 15 0, 16 0, 17 0
 * a door going north should be at 14 18, 15 18, 16 18, 17 18
 */
