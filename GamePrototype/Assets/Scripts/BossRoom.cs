using UnityEngine;
using System.Collections;

public class BossRoom : Room {

	public Vector3 transitionTilePosition;
	public GameObject transitionTile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (numEnemies == 0) {
			transitionTile.transform.position = transitionTilePosition;
			Instantiate (transitionTile);
		}
	}
}
