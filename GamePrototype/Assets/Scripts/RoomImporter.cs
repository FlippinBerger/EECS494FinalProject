using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class RoomImporter : MonoBehaviour {

	private int count = 0;
    private float roomScalar = 1f;

	public TextAsset mapFile; // the name of the file containing the map that will be loaded
	[Serializable]
	// the tile struct contains all the information necessary to place a prefab into the scene
	public struct tile {
		public char character;
		public GameObject prefab;
	}

	public tile[] tileKey; // the actual mappings

    public GameObject floorPrefab; // the prefab that will be used to cover the floor
    public GameObject roomPrefab; // the prefab that will be the parent of all the tiles
    public char doorChar = 'D'; // the character used to place doors
    public int roomWidth; // the width of a room
    public int roomHeight; // the height of a room
    public Direction[] doors;

	// these fields are for faster access to cacheable data
	private Dictionary<char, tile> map = new Dictionary<char, tile>();
	private HashSet<Direction> doorDirections = new HashSet<Direction>();


    private GameObject parentRoom;

	// Use this for initialization
	void Start () {
		// put the tiles into a hashtable for quicker access when building the map
		foreach (tile t in tileKey) {
			this.map[t.character] = t;
		}

        this.parentRoom = Instantiate(this.roomPrefab); // instantiate the parent room

		LoadMapFile(this.mapFile); // load the level into the scene
	}

	// Update is called once per frame
	void Update () {

	}

	// places the character's object into the scene at the specified position
	void PlaceObject(char c, int x, int y) {
        Vector3 pos = new Vector3(roomScalar * x, roomScalar * y);
        if (!map.ContainsKey(c)) { // if the character is unrecognized
			print("Error: unrecognized tile found in map at position: (" + pos.x.ToString() + "," + pos.y.ToString() + ")");
			print (c);
		}
		GameObject prefab = map[c].prefab; // get the prefab corresponding to the character

		if (prefab != null) { // if the prefab is null, place the object
			GameObject obj = Instantiate(prefab); // create the game object in the scene
            if (c == this.doorChar) { // if a door was placed
                if (x == 0) { // if a door to the left
                    obj.GetComponent<Door>().dir = Direction.Left;
                    this.doorDirections.Add(Direction.Left);
                }
                else if (x == this.roomWidth-1) { // if a door to the right
                    obj.GetComponent<Door>().dir = Direction.Right;
                    this.doorDirections.Add(Direction.Right);
                }
                else if (y == 0) { // if a door leading upward
                    obj.GetComponent<Door>().dir = Direction.Up;
                    this.doorDirections.Add(Direction.Up);
                }
                else if (y == this.roomHeight-1) { // if a door leading downward
                    obj.GetComponent<Door>().dir = Direction.Down;
                    this.doorDirections.Add(Direction.Down);
                }
            }

			obj.transform.position = pos; // place the game object in the correct position

            obj.transform.parent = this.parentRoom.transform; // set the tile as a child of the parent room
		}
	}

	string CleanLine(string line) {
		line = line.Trim('\r'); // trim windows newline
		return line;
	}

    // places flooring underneath the entire map
    void PlaceFloor(int width, int height) {
        for (int x = 0; x < width-1; x++) { // horizontal tiling
            for (int y = 0; y < height-1; y++) { // vertical tiling
                Vector3 pos = new Vector3();

                // for each map square
                GameObject floor = Instantiate(this.floorPrefab); // make the floor object

                // scale position based on floor tile size
                pos.x = y * floor.transform.localScale.x * roomScalar;
                pos.y = x * floor.transform.localScale.y * roomScalar;

                floor.transform.position = pos; // place the floor tile

                floor.transform.parent = this.parentRoom.transform; // set the floor as a child of the parent room
            }
        }
    }

    // loads a map into the scene based on its filename
    void LoadMapFile(TextAsset file) {
		try {
			print("Loading map...");
			string[] lines = file.text.Trim().Split('\n'); // split the file into lines
			int height = lines.Length; // the number of tiles on the y axis
			int width = lines[0].Length;

			string line;
			for(int y = height-1; y > -1; y--) {
				line = lines[y];
				if (line.Length != width) {
					print("Error! The line at y=" + y + " has width " + line.Length + ", expected width of " + width + ".");
				}
				line = CleanLine(line); // remove characters not meant to be parsed
				//print("Line " + y.ToString() + ": \"" + line + "\"");
				if (line != null) {
					for (int x = 0; x < line.Length; x++) { // for each character

						//Handle placing doors
						/*
						if(line[x] == 'D'){
							switch(x){
							case x == 0: //Left side door
								if(lines[y + 1][x] == 'D'){ //line previously visited already placed this door
									continue;
								}
								PlaceObject(line[x], x, y);
								break;
							case x == line.Length - 1: //Right side door
								if(lines[y + 1][x] == 'D'){
									continue;
								}
								PlaceObject(line[x], x, y);
								break;
							default: //Door is on top or bot
								if(y == 0){ //Door is bot
									if(lines[y][x - 1] == 'D'){
										continue;
									}
									PlaceObject(line[x], x, y);
								} else if(y == height - 1) { //Door is top
									if(lines[y][x - 1] == 'D'){
										continue;
									}
									PlaceObject(line[x], x, y);
								} else { //Door is neither and we've found an error
									print("We've found a door that isn't on the edge");
								}
								break;
							}
							continue; //Don't need to call PlaceObject because door has been placed already.
						}
						*/
						PlaceObject(line[x], x, y); // place the object corresponding to the character into the scene
						++count;
					}
				}
			}

			PlaceFloor(height, width); // place flooring underneath the map (reversed due to weirdness)
			this.doors = ReadSet(); // set door directions
			print(this.doors.Length);
			Room r = this.parentRoom.AddComponent<Room>();
			r.Init(this.doors);
			print("Map loaded!" + " With count: " + count.ToString());
		}
		catch (Exception e) { // catch exceptions
			Console.WriteLine("{0}\n", e.Message);
		}
	}

	private Direction[] ReadSet(){
		int i = 0;
		Direction[] doors = new Direction[doorDirections.Count];
		foreach (Direction d in doorDirections) {
			doors [i++] = d;
		}
		return doors;
	}
}
