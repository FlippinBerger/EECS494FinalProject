using UnityEngine;
using System.Collections;
using System.Threading;

public class Ladder : MonoBehaviour {

    int entered = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){

        if (Interlocked.Exchange(ref (entered), 1) == 1)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.S.CleanUpGame();
            }
        }
	}
}
