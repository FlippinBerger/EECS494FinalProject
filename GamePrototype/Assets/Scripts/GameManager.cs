﻿using UnityEngine;
using System.Collections;

public enum Direction {
	Up, Down, Left, Right, None
};

public class GameManager : MonoBehaviour {

	static public GameManager S;

    // TODO make an enum and use it to index into this array
    public Sprite[] statusEffectSprites;
    public Sprite[] volcanoSprites;
    public GameObject[] enemyTypes; // TODO split array up according to biome

	//number of players playing the game
	public int numPlayers = 0;
	public Room currentRoom;


	void Awake(){
		S = this;
	}

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
