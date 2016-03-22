using UnityEngine;
using System.Collections;

public class EnemySpawnTile : MonoBehaviour {
    
	void Start() {
        // TODO spawn only neutral enemies or biome-specific enemies
        int index = Random.Range(0, GameManager.S.enemyTypes.Length);
        GameObject enemy = (GameObject)Instantiate(GameManager.S.enemyTypes[index], transform.position, Quaternion.identity);
	}
}
