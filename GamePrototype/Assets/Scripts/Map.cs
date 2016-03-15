using UnityEngine;
using System.Collections;

//Class used to represent a 2D array of Tiles
public class Map : MonoBehaviour {

	public Tile[,] map;
	private int maxSize = 30;

	//Will most likely have player positions on this object as well
	//Player info here

	//Creates the map
	public Map(int rows, int cols){
		map = new Tile[maxSize, maxSize];
		for (int i = 0; i < rows; ++i) {
			for (int j = 0; j < cols; ++j) {
				print ("i: " + i.ToString () + " j: " + j.ToString ());
				map [i,j] = new Tile (TileType.Floor, i, j);
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
