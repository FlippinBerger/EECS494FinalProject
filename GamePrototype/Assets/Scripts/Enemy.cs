using UnityEngine;
using System.Collections;

public class Enemy : Actor {
    public float targetSelectionInterval; // the time interval on which the enemy selects a new target
    public int damage = 1;  // the amount of damage this enemy does to players
    public float knockbackVelocity = 3.0f; // the speed that this enemy knocks players backward
    public float knockbackDuration = 0.1f; // the amount of time this enemy knocks players backward

    private float targetSelectedTimeElapsed = 0.0f; // the time elapsed since last selecting a target
    private GameObject target; // the target the enemy is trying to move toward

    bool elemental = false;

	// Use this for initialization
	new void Start () {
        base.Start(); // call start for actor

        elemental = (Random.Range(0, 2) % 2 == 0); // 50/50 chance of spawning as an elemental enemy
        // set new sprite color


        // start by acquiring a target
        this.targetSelectedTimeElapsed = float.MaxValue;
        this.UpdateTarget();
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 knockbackDirection = this.transform.position - col.gameObject.transform.position; // determine direction of knockback
        if (col.gameObject.tag == "Fireball")
        {
            if (elemental)
            {
                Burn(-1);
            }
            else
            {
                // TODO make these serializable values
                Knockback(5f, knockbackDirection, 0.2f);
                Burn(1);
            }
            Destroy(col.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // do stuff with tiles here, like doors and lava
        if (col.gameObject.tag == "LavaTile")
        {
            if (elemental)
            {
                Burn(-1);
            }
            else
            {
                Burn(1);
                Slow();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "LavaTile")
        {
            UnSlow();
        }
    }

    private void UpdateTarget() {
        /*
        this.targetSelectedTimeElapsed += Time.deltaTime; // update elapsed time

        if (this.targetSelectedTimeElapsed >= this.targetSelectionInterval) { // if it's time to select a new target
            this.targetSelectedTimeElapsed = 0.0f; // reset the elapsed time
            // get the location of the closest player
            GameObject closestPlayer = EnemyAIManager.Instance.players[0];
            float distanceToClosestPlayer = float.MaxValue;
            foreach (GameObject player in EnemyAIManager.Instance.players) {
                float distance = Vector3.Distance(player.transform.position, this.transform.position);
                if (distance < distanceToClosestPlayer) {
                    closestPlayer = player;
                    distanceToClosestPlayer = distance;
                }
            }
            this.target = closestPlayer; // target the closest player
        }
        */
    }

    protected override void UpdateMovement() {
        /*
        Vector3 direction = this.target.transform.position - this.transform.position; // determine the direction of the enemy's target

        if (!this.knockedBack && !this.recoveringFromHit) { // if the enemy is able to move
            this.transform.position += direction * this.moveSpeed * Time.deltaTime; // move toward the target
        }
        */
    }

    // Update is called once per frame
    new void Update() {
        base.Update(); // call update for actor
        UpdateTarget(); // update the target of the enemy
    }
}
