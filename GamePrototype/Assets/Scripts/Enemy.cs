using UnityEngine;
using System.Collections;

public class Enemy : Actor {
    public float targetSelectionInterval; // the time interval on which the enemy selects a new target
    public int damage = 1;  // the amount of damage this enemy does to players
    public float knockbackVelocity = 3.0f; // the speed that this enemy knocks players backward
    public float knockbackDuration = 0.1f; // the amount of time this enemy knocks players backward
    public float aggroDistance; // the distance at which the enemy will start moving toward a player
    public float attackRange; // the max range of this enemy's attack
    public float attackRangeLeeway; // how far within the attackRange the enemy will begin trying to attack (0-1)
    public float attackCooldown; // interval between enemy attacks

    protected float targetSelectedTimeElapsed = 0.0f; // the time elapsed since last selecting a target
    protected float attackCooldownTimeElapsed = 0.0f; // the time elapsed since last attack
    protected GameObject target; // the target the enemy is trying to move toward

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

    protected virtual void Attack(GameObject target) {
        this.attackCooldownTimeElapsed = 0.0f; // reset cooldown
    }

    protected virtual void UpdateAttack() {
        this.attackCooldownTimeElapsed += Time.deltaTime; // update cooldown

        if (this.target == null || this.target.tag != "Player") { // return if target isn't valid
            return;
        }

        if (attackCooldownTimeElapsed < this.attackCooldown) { // return if attack isn't off cooldown
            return;
        }

        float distanceToTarget = (this.target.transform.position - this.transform.position).magnitude;
        if (distanceToTarget <= this.attackRange * this.attackRangeLeeway) { // if the enemy is close enough to attack the player (with some leeway)
            Attack(this.target);
        }

    }

    protected override void UpdateMovement() { 
        Vector3 direction = this.target.transform.position - this.transform.position; // determine the direction of the enemy's target

        bool ableToMove = (!this.knockedBack && !this.recoveringFromHit); // if there is nothing preventing the player from moving
        float distanceToTarget = (this.target.transform.position - this.transform.position).magnitude;
        bool closeEnough = (distanceToTarget <= this.attackRange * this.attackRangeLeeway); // if the enemy is as close as it needs to be to attack
        if (ableToMove && !closeEnough) { // if the enemy is able and willing to move
            this.transform.position += direction * this.moveSpeed * Time.deltaTime; // move toward the target
        }
    }

    // Update is called once per frame
    new void Update() {
        base.Update(); // call update for actor
        UpdateTarget(); // update the target of the enemy
        UpdateAttack(); // consider whether or not to attack
    }
}
