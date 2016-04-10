using UnityEngine;
using System.Collections;

public class EnemySpawnTile : MonoBehaviour {
    
	void Start() {

        GameObject parentRoom = transform.parent.gameObject;

        //spawn random enemy
        int index = Random.Range(0, GameManager.S.enemyTypes.Length); // randomly select an enemy type
        GameObject enemyGO = (GameObject)Instantiate(GameManager.S.enemyTypes[index], transform.position, Quaternion.identity); // create the chosen enemy
        enemyGO.transform.parent = parentRoom.transform; // set the enemy as part of their parent room
        enemyGO.GetComponent<Enemy>().homeTile = this.gameObject; // set the enemy's home tile

        parentRoom.GetComponent<Room>().AddEnemy(enemyGO);
    }
}
