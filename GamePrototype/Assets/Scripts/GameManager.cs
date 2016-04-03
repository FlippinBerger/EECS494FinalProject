using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Direction {
	Up, Down, Left, Right, None
};

public enum Element
{
    Fire, Ice, None
}

//Create a Level GO to act as a parent to be deleted at the end of each level in order to clean up memory

public class GameManager : MonoBehaviour {

	static public GameManager S;

    // TODO make an enum and use it to index into this array
    public Sprite[] statusEffectSprites;
    public Sprite[] liquidTileSprites;
    public Sprite[] volcanoSprites;
    public GameObject[] enemyTypes; // TODO split array up according to biome

	public GameObject door;
	public GameObject hallway;

	//Room data
	public TextAsset[] layoutFiles;
	public TextAsset[] roomFiles;
	public TextAsset[] bossRoomFiles; //Indexed by Element enum
	public int roomWidth = 24;
	public int roomHeight = 16;
	public int hallLength = 8;
	public int hallWidth = 4;

	//Door offsets
	public int h_UpAndDown = 14;
	public int v_LeftAndRight = 7;

	//Game meta data
	public int numPlayers = 0;
	public Room currentRoom;

	//Parent Level Prefab to be used to clean up at the end of a level
	public GameObject levelGO;

	//Returns a random element from the enum to be used in each dungeon level
	public Element GetRandomElement(){
        // return UnityEngine.Random.Range (0, Enum.GetNames(typeof(Element)).Length);
        return Element.Fire;
	}

	public TextAsset GetRandomRoomFile(){
		return roomFiles[UnityEngine.Random.Range(0, roomFiles.Length)];
	}

	void Awake(){
		S = this;
	}

	//Use this function to set up the initial game level
	//TODO Eventually create a start screen instead of just launching the game
	//     in order to keep players from being shocked
	void Start(){
		DungeonLayoutGenerator.S.CreateLevelMap ();

		DungeonLayout DL = DungeonLayoutGenerator.S.levelLayout.GetComponent<DungeonLayout> ();
		CameraController.S.SetCameraPosition(DL.startRoomPosition); //set the initial camera position
		
	}
}
