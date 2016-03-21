﻿using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
    public int health; // the amount of damage the actor can take before dying
    public float hitRecoveryTime; // the time the actor spends invulnerable after being hit
    public float hitFlashInterval; // the amount of time in between color flashes when hit
    public Color flashColor = Color.red; // the color the sprite will flash when hit
    public Color spriteColor = Color.grey; // the color of the default sprite
    public float moveSpeed; // the movement speed of this enemy

    protected float recoveryTimeElapsed = 0.0f; // the time elapsed since hit
    protected bool knockedBack = false; // whether the enemy is currently knocked back or not
    protected bool recoveringFromHit = false; // whether the enemy is recovering or not

    // Use this for initialization
    protected void Start () {
        this.GetComponent<SpriteRenderer>().color = this.spriteColor; // set the sprite's color
        this.recoveryTimeElapsed = this.hitRecoveryTime; // don't start by being invulnerable
    }

    public void Hit(int damage, float knockbackVelocity, Vector2 knockbackDirection, float knockbackDuration) {
        if (damage <= 0 || this.recoveryTimeElapsed < this.hitRecoveryTime) { // if no damage was dealt, or if the actor is invulerable
            return; // do nothing
        }

        this.health -= damage; // take damage

        Knockback(knockbackVelocity, knockbackDirection, knockbackDuration); // knock the enemy backward

        if (this.health <= 0) { // check for death
            Die();
        }

        this.StartFlashing(); // indicate damage by flashing
    }

    protected void Knockback(float knockbackValue, Vector2 knockbackDirection, float knockbackDuration) {
        knockbackDirection.Normalize(); // normalize the direction
        this.GetComponent<Rigidbody2D>().velocity = (knockbackDirection * knockbackValue); // apply the knockback force
        this.knockedBack = true; // set the knockback flag
        Invoke("StopKnockback", knockbackDuration); // stop the knockback

    }

    // reset velocity to zero
    protected void StopKnockback() {
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // stop motion
        this.knockedBack = false; // mark as not being knocked back
    }

    protected virtual void Die() {
        Destroy(this.gameObject);
    }

    protected void StartFlashing() {
        this.recoveryTimeElapsed = 0.0f; // reset recovery time elapsed
    }

    protected void UpdateRecovery() {
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

    protected virtual void UpdateMovement() {
        return;
    }

    protected void Update() {
        if (!knockedBack) {
            UpdateMovement();
        }
        UpdateRecovery();
    }
}
