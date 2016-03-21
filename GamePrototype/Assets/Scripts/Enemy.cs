using UnityEngine;
using System.Collections;

public class Enemy : Actor {
    public float targetSelectionInterval; // the time interval on which the enemy selects a new target
    public int damage = 1;  // the amount of damage this enemy does to players
    public float knockbackVelocity = 3.0f; // the speed that this enemy knocks players backward
    public float knockbackDuration = 0.1f; // the amount of time this enemy knocks players backward

    private float targetSelectedTimeElapsed = 0.0f; // the time elapsed since last selecting a target
    private GameObject target; // the target the enemy is trying to move toward

	// Use this for initialization
	new void Start () {
        base.Start(); // call start for actor

        // start by acquiring a target
        this.targetSelectedTimeElapsed = float.MaxValue;
        this.UpdateTarget();
	}

    private void UpdateTarget() {
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
