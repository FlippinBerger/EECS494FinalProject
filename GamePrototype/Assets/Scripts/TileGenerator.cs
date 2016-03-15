using UnityEngine;
using System.Collections;

public class TileGenerator : MonoBehaviour {

	static public TileGenerator S;

	//Different Types of Sprites for the tiles to use
	public Sprite floorTile1;
	public Sprite wallTile1;

	//method that sets the Sprite for the given TileType
	public Sprite SetTileSprite(TileType tt){
		switch (tt) {
		case TileType.Floor:
			return floorTile1;
		case TileType.Wall:
			return wallTile1;
		default:
			return null;
		}
	}

	void Awake(){
		S = this;
	}
}
