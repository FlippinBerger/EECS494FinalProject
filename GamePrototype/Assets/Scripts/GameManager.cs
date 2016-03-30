using UnityEngine;
using System.Collections;

public enum Direction {
	Up, Down, Left, Right, None
};

public enum Element
{
    Fire, Ice
}

//Create a Level GO to act as a parent to be deleted at the end of each level in order to clean up memory

public class GameManager : MonoBehaviour {

	static public GameManager S;

    // TODO make an enum and use it to index into this array
    public Sprite[] statusEffectSprites;
    public Sprite[] liquidTileSprites;
    public Sprite[] volcanoSprites;
    public GameObject[] enemyTypes; // TODO split array up according to biome

	//number of players playing the game
	public int numPlayers = 0;
	public Room currentRoom;


	void Awake(){
		S = this;
	}
}
