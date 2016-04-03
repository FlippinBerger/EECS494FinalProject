using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class RoomImporter : MonoBehaviour {

	public static RoomImporter S;

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

    // public GameObject floorPrefab;
    public GameObject[] hazardTilePrefabs;
    // public char doorChar = 'D'; // the character used to place doors
    public int roomWidth; // the width of a room
    public int roomHeight; // the height of a room
    public Direction[] doors;
    public Element element;

	// these fields are for faster access to cacheable data
	private Dictionary<char, tile> map = new Dictionary<char, tile>();
	private HashSet<Direction> doorDirections = new HashSet<Direction>();


    private GameObject parentRoom;
    //Element currentElement;

	void Awake(){
		S = this;
	}

	// Use this for initialization
	void Start () {
		// put the tiles into a hashtable for quicker access when building the map
		foreach (tile t in tileKey) {
			map[t.character] = t;
		}
        // CreateRoom(mapFile, element);
    }

    public GameObject CreateRoom(TextAsset file, Element elt)
    {
        // do stuff
        parentRoom = new GameObject("Room"); // instantiate the parent room
        element = elt;

        // jank
        tile t;
        t.character = 'H';
        t.prefab = hazardTilePrefabs[(int)element];
        map['H'] = t;

        LoadMapFile(file); // load the level into the scene
        return parentRoom;
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

            switch (c)
            {
                case ' ':
                case 'E':
				obj.GetComponent<SpriteRenderer>().sprite = GameManager.S.floorTileSprites[(int)element];
                    break;
                case 'L':
                    obj.GetComponent<LiquidTile>().SetElement(element);
                    break;
                case 'H':

                    break;
            }

			obj.transform.position = pos; // place the game object in the correct position

            obj.transform.parent = this.parentRoom.transform; // set the tile as a child of the parent room
		}
	}

	string CleanLine(string line) {
		line = line.Trim('\r'); // trim windows newline
		return line;
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
						PlaceObject(line[x], x, y); // place the object corresponding to the character into the scene
						++count;
					}
				}
			}
            
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
