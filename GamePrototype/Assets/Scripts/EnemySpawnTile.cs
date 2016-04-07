using UnityEngine;
using System.Collections;

public class EnemySpawnTile : MonoBehaviour {
    
	void Start() {

        GameObject parentRoom = transform.parent.gameObject;
        parentRoom.GetComponent<Room>().AddEnemy();

        //spawn random enemy
        int index = Random.Range(0, GameManager.S.enemyTypes.Length);
        GameObject enemyGO = (GameObject)Instantiate(GameManager.S.enemyTypes[index], transform.position, Quaternion.identity);
        enemyGO.transform.parent = parentRoom.transform;
	}
}
