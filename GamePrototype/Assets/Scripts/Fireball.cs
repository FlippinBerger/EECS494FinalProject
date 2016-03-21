using UnityEngine;
using System.Collections;

// TODO make it so fireballs explode on contact with players/certain enemies/walls

public class Fireball : MonoBehaviour {

    public float speed = 1f;

    GameObject dangerIndicator = null;
    GameObject sprite;

    float startTime;
    float journeyLength;
    Vector3 startLocation;
    Vector3 destination;
    bool moving = false;

    void Awake()
    {
        sprite = transform.FindChild("Sprite").gameObject;
    }
	
	void Update () {
        // lerp towards dest
        if (moving)
        {
            //Vector3 dir = destination;
            //sprite.transform.rotation = Quaternion.LookRotation(dir);
             
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startLocation, destination, fracJourney);

            if (fracJourney >= 1)
            {
                //explode
                if (dangerIndicator != null) Destroy(dangerIndicator);
                Destroy(this.gameObject);
            }
        }
    }

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        moving = true;
        startLocation = transform.position;
        startTime = Time.time;
        journeyLength = (destination - transform.position).magnitude;
    }

    // want this to be separate from SetDestination in case we ever have fireballs without danger indicators
    // we could merge them if we want all fireballs to have a danger indicator
    public void SetAssociatedDangerIndicator(GameObject danger)
    {
        dangerIndicator = danger;
    }
}
