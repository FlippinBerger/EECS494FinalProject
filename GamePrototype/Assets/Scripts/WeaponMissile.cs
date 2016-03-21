﻿using UnityEngine;
using System.Collections;

public class WeaponMissile : Weapon {

    public float missileSpeed; // the traveling speed of the missile

    private float swordRotationAngle = 0f; // the current rotation of the sword in degrees relative to the player

    // Use this for initialization
    void Start() {
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.localPosition = Vector3.up * 0.7f; // spawn the missile relative to the player
        this.transform.rotation = Quaternion.AngleAxis(this.parentPlayer.playerRotationAngle, Vector3.forward); // create a rotation that can be applied to a vector
        this.gameObject.GetComponent<Rigidbody2D>().velocity = this.transform.rotation * Vector3.up * missileSpeed; // set the velocity of the missile
        this.transform.parent = null; // remove the player as the parent

        this.parentPlayer.StopAttack(this.cooldown); // stop attacking and set the player on cooldown
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") { // if the sword hits an enemy
            Vector2 knockbackDirection = col.transform.position - this.parentPlayer.transform.position; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Hit(this.damage, this.knockbackVelocity, knockbackDirection, this.knockbackDuration); // deal damage to the enemy
        }
        Destroy(this.gameObject); // destroy on collision
    }

    void Update() {

    }
}
