using UnityEngine;
using System.Collections;

// TODO make it so fireballs explode on contact with players/certain enemies/walls

public class Fireball : Hazard {

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

    void OnDestroy()
    {
        if (dangerIndicator != null) Destroy(dangerIndicator);
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else
        {
            Vector2 knockbackDirection = col.gameObject.transform.position - transform.position; // determine direction of knockback
            if (col.collider.tag == "Player" || col.collider.tag == "Enemy")
            {
                Actor actor = col.gameObject.GetComponent<Actor>();
                actor.Hit(new AttackHitInfo(0, knockbackVelocity, knockbackDuration, element, GameManager.S.round, this.gameObject), knockbackDirection);
                Destroy(gameObject);
            }
        }
    }
	
	void Update () {
        // lerp towards dest
        if (moving)
        {
            Vector3 dir = destination - transform.position;
            sprite.transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
             
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startLocation, destination, fracJourney);

            if (fracJourney >= 1)
            {
                //explode
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
