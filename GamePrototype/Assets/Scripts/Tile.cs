using UnityEngine;
using System.Collections;

public enum TileType {
	Floor,
	Wall
};

public class Tile : MonoBehaviour {

	//dimensions of the sqare tile
	static public float size = 5.0f;

	public Sprite tileSprite;
	public Vector2 pos;
	public TileType tileType;

	public Tile(TileType tt, int row, int col){
		tileType = tt;
		//pos = new Vector2 (col * size, row * size);
		pos = new Vector2(col, row - 1);
		tileSprite = TileGenerator.S.SetTileSprite (tt);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
