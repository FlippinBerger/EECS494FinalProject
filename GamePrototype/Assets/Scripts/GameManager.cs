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
    
    // Sprites/Colors
    public Sprite[] statusEffectSprites;
    public Sprite[] liquidTileSprites;
    public Sprite[] volcanoSprites;
    public Color[] elementColors;
	public Sprite[] floorTileSprites; //Sprites to be used for placing any floor tiles on the fly
    public Sprite playerSprite;
    public Sprite tombstoneIcon;

    // Random Prefabs
    public GameObject[] enemyTypes;
    public GameObject[] weaponDrops;
    public GameObject weaponPickupPrefab;
    public GameObject floorTile; //Floor tile prefab used to place floor tiles on the fly
    public GameObject wallTile; //Wall tile prefab ^^
    public GameObject door;
    public GameObject coinPrefab;
	public GameObject wallFixture; //Used to place walls where doors aren't needed
	public GameObject hallway;

	//Players
	public GameObject playerPrefab;
	public GameObject[] players;
	public bool playersInitialized = false;

	//Room data
	public TextAsset[] layoutFiles;
	public TextAsset[] roomFiles;
	public TextAsset bossRoomFile;
	private List<TextAsset> layoutList;
	private List<TextAsset> roomList;

	public TextAsset[] bossRoomFiles; //Indexed by Element enum
	public int roomWidth = 24;
	public int roomHeight = 16;
	public int hallLength = 8;
	public int hallWidth = 6;

	//Door offsets
	public int h_UpAndDown = 13;
	public int v_LeftAndRight = 6;

    //Game meta data
    public GameObject HUDCanvas;
	public int numPlayers = 0;
    public int numRounds = 50;
	public Room currentRoom;
	public Element currentLevelElement;
	int round = 0;

	//GameObjects that need to be deleted after the level is finished
	List<GameObject> levelGOs;

	void Awake(){
		S = this;
	}

	//Use this function to set up the initial game level
	//TODO Eventually create a start screen instead of just launching the game
	//     in order to keep players from being shocked
	void Start(){
		Setup (); //Only called when the game actually starts up

		CreateDungeonLevel ();
		//Create Players and set their position
		players = new GameObject[numPlayers];
		for (int i = 1; i <= numPlayers; ++i) {
			GameObject p = Instantiate (playerPrefab);
			Player player = p.GetComponent<Player> ();
			player.playerNum = i;
			player.controllerNum = 0;
			player.PlacePlayer();
			players [i - 1] = p;
		}
		playersInitialized = true;
	}


	//Setup functions for the GameManager

	//Does a lot of book keeping set up for the game
	//Also initialized structures for the GameManager
	void Setup(){
		//init structures
		levelGOs = new List<GameObject>();
		layoutList = new List<TextAsset> ();
		roomList = new List<TextAsset> ();

		//Fill structures
		LoadTextAssets (); //load text files
	}

	void LoadTextAssets(){
		// string path = Application.dataPath;
		foreach(TextAsset ta in Resources.LoadAll("LayoutFiles", typeof(TextAsset))){
			layoutList.Add(ta);
		}
		GameManager.S.layoutFiles = layoutList.ToArray ();

		foreach (TextAsset ta in Resources.LoadAll("RoomFiles", typeof(TextAsset))) {
			roomList.Add (ta);
		}
		GameManager.S.roomFiles = roomList.ToArray ();
	}



	//Game Helpers

	//Adds child to the level game object to be deleted at the end of the level
	public void AddObject(GameObject go){
		levelGOs.Add (go);
	}

	//Returns a random element from the enum to be used in each dungeon level
	public Element GetRandomElement(){
		Element newElt = Element.None;
		do {
			newElt = (Element)UnityEngine.Random.Range (0, Enum.GetNames (typeof(Element)).Length - 1);
		} while (newElt == currentLevelElement);

		return newElt;
	}

	//Returns a random room file to be placed in the level
	public TextAsset GetRandomRoomFile(){
		return roomFiles[UnityEngine.Random.Range(0, roomFiles.Length)];
	}


	//Level Creation Methods

	//Picks an element for the Level, Creates the map, Creates a Dungeon Layout, and sets the initial game state for that level
	void CreateDungeonLevel(){
		currentLevelElement = GetRandomElement(); //set the initial element for this dungeon level
        HUDCanvas.transform.FindChild("CurrentLevel").GetComponent<UnityEngine.UI.Text>().text = "Level " + (round + 1);
		DungeonLayoutGenerator.S.CreateLevelMap();
		DungeonLayout DL = DungeonLayoutGenerator.S.levelLayout.GetComponent<DungeonLayout> ();
		//CameraController.S.SetCameraPosition(DL.startRoomPosition); //set the initial camera position
	}

	//Level Destruction
	public void CleanUpGame(){
		++round;

		foreach (GameObject go in levelGOs) {
			Destroy (go);
		}

		if (round < numRounds) {
			CreateDungeonLevel ();
			foreach (GameObject p in players) {
				Player player = p.GetComponent<Player> ();
				player.PlacePlayer ();
			}
		} else {
			//GG show an end screen or something here
			return;
		}
	}


	//End Game Functions

	//Function that checks to see if all the players died
	public void CheckPlayers(){
		bool allDead = true;
		foreach (GameObject go in players) {
			Player p = go.GetComponent<Player> ();
			if (!p.dead) {
				allDead = false;
				break;
			}
		}

		if (allDead) {
			RestartPrompt ();
		}
	}

	//UI Functions
	void RestartPrompt(){
		return;
	}
}
