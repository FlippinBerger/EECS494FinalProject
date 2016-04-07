using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

	public bool currentRoom = false;
    int enemyCount = 0;

    public void AddEnemy()
    {
        ++enemyCount;
    }

    public void RemoveEnemy()
    {
        --enemyCount;
        // if enemycount == 0  open doors
        if (enemyCount == 0)
        {
            print("ayy");
        }
    }
}
