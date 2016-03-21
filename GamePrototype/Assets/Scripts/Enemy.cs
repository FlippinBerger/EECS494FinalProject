using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public int health = 2; // the amount of damage the enemy can take before dying
    public float hitRecoveryTime = 1f; // the time the enemy spends invulnerable after being hit
    public float hitFlashInterval = 0.2f; // the amount of time in between color flashes when hit
    public Color flashColor = Color.red; // the color the sprite will flash when hit
    public Color spriteColor = Color.grey; // the color of the default sprite
    public float moveSpeed; // the movement speed of this enemy
    public float targetSelectionInterval; // the time interval on which the enemy selects a new target

    private float targetSelectedTimeElapsed = 0.0f; // the time elapsed since last selecting a target
    private float recoveryTimeElapsed = 0.0f; // the time elapsed since hit
    private bool knockedBack = false; // whether the enemy is currently knocked back or not
    private bool recoveringFromHit = false; // whether the enemy is recovering or not
    private GameObject target; // the target the enemy is trying to move toward

	// Use this for initialization
	void Start () {
        this.GetComponent<SpriteRenderer>().color = this.spriteColor; // set the sprite's color
        this.recoveryTimeElapsed = this.hitRecoveryTime; // don't start by being invulnerable
        this.targetSelectedTimeElapsed = float.MaxValue; // start by acquiring a target
	}

    public void Hit(int damage, float knockbackVelocity, Vector2 knockbackDirection, float knockbackDuration) {
        if (damage <= 0 || this.recoveryTimeElapsed < this.hitRecoveryTime) { // if no damage was dealt, or if the enemy is invulerable
            return; // do nothing
        }

        this.health -= damage; // take damage

        Knockback(knockbackVelocity, knockbackDirection, knockbackDuration); // knock the enemy backward

        if (this.health <= 0) { // check for death
            Die();
        }

        this.StartFlashing(); // indicate damage by flashing
    }

    private void Knockback(float knockbackValue, Vector2 knockbackDirection, float knockbackDuration) {
        knockbackDirection.Normalize(); // normalize the direction
        this.GetComponent<Rigidbody2D>().velocity = (knockbackDirection * knockbackValue); // apply the knockback force
        this.knockedBack = true; // set the knockback flag
        Invoke("StopKnockback", knockbackDuration); // stop the knockback

    }

    // reset velocity to zero
    private void StopKnockback() {
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // stop motion
        this.knockedBack = false; // mark as not being knocked back
    }

    private void StartFlashing() {
        this.recoveryTimeElapsed = 0.0f; // reset recovery time elapsed
    }

    private void Die() {
        Destroy(this.gameObject);
    }

    private void UpdateRecovery() {
        if (this.recoveryTimeElapsed < this.hitRecoveryTime) { // if currently recovering from a hit
            this.recoveringFromHit = true; // set flag
            this.recoveryTimeElapsed += Time.deltaTime; // update elapsed time
            int flashType = (int)(this.recoveryTimeElapsed / this.hitFlashInterval); // get an int to represent flash state
            if (flashType % 2 == 0) { // if flash state is even
                this.GetComponent<SpriteRenderer>().color = this.flashColor; // flash damage color
            }
            else {
                this.GetComponent<SpriteRenderer>().color = this.spriteColor; // flash the normal color
            }
        }

        if (this.recoveryTimeElapsed >= this.hitRecoveryTime) { // if the enemy is no longer recovering from a hit
            this.GetComponent<SpriteRenderer>().color = this.spriteColor; // return to its normal color
            this.recoveringFromHit = false; // set flag
        }
    }

    private void UpdateMovement() {
        Vector3 direction = this.target.transform.position - this.transform.position; // determine the direction of the enemy's target

        if (!this.knockedBack && !this.recoveringFromHit) { // if the enemy is able to move
            this.transform.position += direction * this.moveSpeed * Time.deltaTime; // move toward the player
        }
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
	
	// Update is called once per frame
	void Update() {
        UpdateRecovery(); // manage flashing effect and hit recovery
        UpdateTarget(); // update the target of the enemy
        UpdateMovement(); // manage movement
    }
}
