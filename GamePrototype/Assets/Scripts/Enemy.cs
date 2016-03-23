using UnityEngine;
using System.Collections;

public class Enemy : Actor {
    public float targetSelectionInterval; // the time interval on which the enemy selects a new target
    public int damage = 1;  // the amount of damage this enemy does to players
    public float knockbackVelocity = 3.0f; // the speed that this enemy knocks players backward
    public float knockbackDuration = 0.1f; // the amount of time this enemy knocks players backward
    public float aggroDistance; // the distance at which the enemy will start attacking a player

    private float targetSelectedTimeElapsed = 0.0f; // the time elapsed since last selecting a target
    private GameObject target; // the target the enemy is trying to move toward

	// Use this for initialization
	protected override void Start () {
        // start by acquiring a target
        this.targetSelectedTimeElapsed = targetSelectionInterval + 1f;
        this.UpdateTarget();

        base.Start(); // call start for actor
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 knockbackDirection = this.transform.position - col.gameObject.transform.position; // determine direction of knockback
        if (col.gameObject.tag == "Hazard")
        {
            Hazard hazard = col.gameObject.GetComponent<Hazard>();
            // TODO make these serializable values
            Knockback(hazard.knockbackVelocity, knockbackDirection, hazard.knockbackDuration);
            Burn(1);
            Destroy(col.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // do stuff with tiles here, like doors and lava
        if (col.gameObject.tag == "LavaTile")
        {
            Burn(1);
            Slow();
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
        
        this.targetSelectedTimeElapsed += Time.deltaTime; // update elapsed time
        
        if (!recoveringFromHit) { // if it's time to select a new target
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
            if (distanceToClosestPlayer <= this.aggroDistance) { // if a player is within aggro distance
                this.target = closestPlayer; // target the closest player
            }
            else {
                this.target = this.gameObject; // sit still
            }
        }
       
    }

    protected override void UpdateMovement() { 
        Vector3 direction = this.target.transform.position - this.transform.position; // determine the direction of the enemy's target

        if (!this.knockedBack && !this.recoveringFromHit) { // if the enemy is able to move
            this.transform.position += direction * this.moveSpeed * Time.deltaTime; // move toward the target
        }
    }

    // Update is called once per frame
    new void Update() {
        base.Update(); // call update for actor
        UpdateTarget(); // update the target of the enemy
    }
}
