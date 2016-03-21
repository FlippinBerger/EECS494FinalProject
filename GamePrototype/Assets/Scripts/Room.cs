using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

	public Enemy[] enemies;
	public Door[] doors;

	public int numEnemies;

	// Use this for initialization
	void Start () {
		numEnemies = enemies.Length;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
