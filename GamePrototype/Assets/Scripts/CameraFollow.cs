using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public bool allDead = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.S.playersInitialized)
        {
            Player p1 = GameManager.S.players[0].GetComponent<Player>();
            Player p2 = GameManager.S.players[1].GetComponent<Player>();
            Vector3 p1Pos = p1.gameObject.transform.position;
			Vector3 p2Pos = p2.gameObject.transform.position;
            Vector3 cameraPos = transform.position;

            // jank
            if (p1.dead && !p2.dead)
            {
                cameraPos = p2Pos;
            }
            else if (p2.dead && !p1.dead)
            {
                cameraPos = p1Pos;
            }
            else if (!p1.dead && !p2.dead)
            {
                cameraPos = Vector3.Lerp(p1Pos, p2Pos, .5f); // midpoint
            }

			cameraPos.z = -10;
			gameObject.transform.position = cameraPos;

            /*
			Vector3 pos = GameManager.S.players [0].transform.position;
			pos.z = -10f;
			gameObject.transform.position = pos;
            */
		}
	}
}
