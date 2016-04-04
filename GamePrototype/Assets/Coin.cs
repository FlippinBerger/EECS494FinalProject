using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public float lifetime = 10f;
    float spawntime;

	// Use this for initialization
	void Start () {
        spawntime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float elapsedTime = Time.time - spawntime;
        if (elapsedTime > lifetime * (4 / 5))
        {
            // flicker and fade
        }
	    if (elapsedTime > lifetime)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            // increment gold
            Destroy(gameObject);
        }
    }
}
