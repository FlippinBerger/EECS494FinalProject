using UnityEngine;
using System.Collections;
using System.Threading;

public class Ladder : MonoBehaviour {

    int entered = 0;

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        { 
            if (Interlocked.Exchange(ref (entered), 1) == 0)
            {
                GameManager.S.CleanUpGame();
            }
        }
	}
}
