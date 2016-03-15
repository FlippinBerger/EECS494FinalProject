using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public int health = 2; // the amount of damage the enemy can take before dying
    public float hitRecoveryTime = 1f; // the time the enemy spends invulnerable after being hit
    public float hitFlashInterval = 0.2f; // the amount of time in between color flashes when hit
    public Color flashColor = Color.red; // the color the sprite will flash when hit
    public Color spriteColor = Color.grey; // the color of the default sprite

    private float recoveryTimeElapsed = 0.0f; // the time elapsed since hit

	// Use this for initialization
	void Start () {
        this.GetComponent<SpriteRenderer>().color = this.spriteColor; // set the sprite's color
        this.recoveryTimeElapsed = this.hitRecoveryTime; // don't start by being invulnerable
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
        Invoke("StopMoving", knockbackDuration);

    }

    // reset velocity to zero
    private void StopMoving() {
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    private void StartFlashing() {
        this.recoveryTimeElapsed = 0.0f; // reset recovery time elapsed
    }

    private void Die() {
        Destroy(this.gameObject);
    }
	
	// Update is called once per frame
	void Update() {
	    if (this.recoveryTimeElapsed < this.hitRecoveryTime) { // if currently recovering from a hit
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
        }

    }
}
