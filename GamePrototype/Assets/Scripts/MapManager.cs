using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class MapManager : MonoBehaviour {

	private int count = 0;

	public TextAsset mapFile; // the name of the file containing the map that will be loaded
	[Serializable]
	// the tile struct contains all the information necessary to place a prefab into the scene
	public struct tile {
		public char character;
		public GameObject prefab;
	}

	public tile[] tileKey; // the actual mappings

	/*
	public GameObject floorPrefab; // the prefab used for flooring
	public GameObject wallPrefab; //the prefab used for walls
	*/

	// these fields are for faster access to cacheable data
	private Dictionary<char, tile> map = new Dictionary<char, tile>();

	// Use this for initialization
	void Start () {
		// put the tiles into a hashtable for quicker access when building the map
		foreach (tile t in tileKey) {
			this.map[t.character] = t;
		}

		LoadMapFile(this.mapFile); // load the level into the scene
	}

	// Update is called once per frame
	void Update () {

	}

	// places the character's object into the scene at the specified position
	void PlaceObject(char c, Vector3 pos) {
		if (!map.ContainsKey(c)) { // if the character is unrecognized
			print("Error: unrecognized tile found in map at position: (" + pos.x.ToString() + "," + pos.y.ToString() + ")");
			print (c);
		}
		GameObject prefab = map[c].prefab; // get the prefab corresponding to the character

		if (prefab != null) { // if the prefab is null, place the object
			GameObject obj = Instantiate(prefab); // create the game object in the scene

			/*
			// determine the height of the object for correct y placement
			Vector3 bounds = obj.GetComponent<Renderer>().bounds.size; // get object bounds
			pos.y = bounds.y / 2.0f;// determine y placement

			pos.x *= obj.transform.localScale.x;
			pos.z *= obj.transform.localScale.z;
			*/

			obj.transform.position = pos; // place the game object in the correct position
		}
	}

	//Don't think we need to place floors seperately from the main map generation for a 2D map
	/*
	// places flooring underneath the entire map
	void PlaceFloor(int width, int height) {
		for (int x = 0; x < width; x++) { // horizontal tiling
			for (int y = 0; y < height; y++) { // vertical tiling
				Vector3 pos = new Vector3();

				// for each map square
				GameObject floor = Instantiate(this.floorPrefab); // make the floor object

				// determine the height of the object for correct y placement
				Vector3 floorBounds = floor.GetComponent<Renderer>().bounds.size; // get floor bounds
				pos.y = 0f - (floorBounds.y / 2f); // determine y placement

				// scale position based on floor tile size
				pos.x = x * floor.transform.localScale.x;
				pos.z = y * floor.transform.localScale.z;

				floor.transform.position = pos; // place the floor tile
			}
		}
	}
	*/

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
						//Vector3 pos = new Vector3(y, 0f, x); // create a position vector based on its position in the text file (reversed due to weirdness)
						Vector3 pos = new Vector3(.64f * x, .64f * y);
						if (line[x] == '.') { // if this is part of a larger prefab
							; // do nothing
						}
						else {
							PlaceObject(line[x], pos); // place the object corresponding to the character into the scene
							++count;
						}
					}
				}
			}

			//PlaceFloor(height, width); // place flooring underneath the map (reversed due to weirdness)
			print("Map loaded!" + " With count: " + count.ToString());
		}
		catch (Exception e) { // catch exceptions
			Console.WriteLine("{0}\n", e.Message);
		}
	}
}
