using UnityEngine;
using System.Collections;

public class EnemyAIManager : MonoBehaviour {
    public static EnemyAIManager Instance;
    public GameObject[] players; // the players in the scene

	// Use this for initialization
	void Start () {
        // if the singleton exists
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this; // set singleton instance

        this.players = GameObject.FindGameObjectsWithTag("Player"); // find positions of players
    }
	
	// Update is called once per frame
	void Update () {
        this.players = GameObject.FindGameObjectsWithTag("Player"); // find positions of players
    }
}
