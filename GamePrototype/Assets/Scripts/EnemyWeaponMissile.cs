using UnityEngine;
using System.Collections;
using System;

public class EnemyWeaponMissile : EnemyWeapon {

    [Header("Missile Base Attributes")]
    public float projectileSpeed; // the travel speed of the projectile
    [Header("Missile Scaling Attributes")]
    public float projectileSpeedScalingFactor;


    // Use this for initialization
    protected override void Start() {
        base.Start(); // set weapon attributes

        this.parentEnemy = this.transform.parent.gameObject.GetComponent<Enemy>(); // set the parent enemy
        Vector3 vectorToPlayer = (this.target.transform.position - this.parentEnemy.transform.position).normalized * this.parentEnemy.attackRange; // get vector from enemy to player
        this.transform.localPosition = transform.localRotation * vectorToPlayer.normalized * 0.1f; // spawn the missile relative to the enemy and the player
        float angle = Mathf.Atan2(vectorToPlayer.x, vectorToPlayer.y) * Mathf.Rad2Deg * -1f; // get the angle toward the player
        this.transform.rotation = Quaternion.Euler(0, 0, angle); // update the missile's rotation
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed; // start the missile's travel
    }

    public virtual void OnTriggerStay2D(Collider2D col) {
        if (col.gameObject.tag == "Player") {
            Destroy(this.gameObject); // destroy on collision with player
        }
        else if (col.tag == "Wall" || col.tag == "Door") {
            Destroy(this.gameObject); // destroy on collision with wall
        }
    }
}
