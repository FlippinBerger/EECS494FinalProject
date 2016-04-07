using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {
    public float cameraZPos = -10; // the z position of the camera

	Player[] GetPlayers() {
        // get all players in the game
        Player [] playersToFollow = new Player[GameManager.S.numPlayers];

        for (int i = 0; i < playersToFollow.Length; i++) {
            playersToFollow[i] = GameManager.S.players[i].GetComponent<Player>();
        }

        return playersToFollow;
	}

    Vector3 MeanVector(List<Vector3> vectors)
    {
        Vector3 tempVector = new Vector3(0, 0, 0);
        foreach (Vector3 vector in vectors)
        {
            tempVector += vector;
        }

        tempVector = tempVector / (float)vectors.Count;

        return tempVector;
    }
	
	// Update is called once per frame
	void Update () {
		if (GameManager.S.playersInitialized)
        {
            Player[] playersToFollow = GetPlayers();
            // get all the vectors
            List<Vector3> vectors = new List<Vector3>();
            for (int i = 0; i < playersToFollow.Length; i++)
            {
                if (!playersToFollow[i].dead) // if the player is alive
                {
                    vectors.Add(playersToFollow[i].gameObject.transform.position); // track the player's position
                }
            }

            Vector3 cameraPos = MeanVector(vectors); // find the mean vector of player positions
            cameraPos.z = this.cameraZPos; // set the camera's z position
			gameObject.transform.position = cameraPos; // apply changes to the camera GameObject
		}
	}
}
