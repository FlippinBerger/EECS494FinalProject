using UnityEngine;
using System.Collections;
using System;

public class EnemyWeaponMissile : EnemyWeapon {

    [Header("Missile Base Attributes")]
    public float projectileSpeed; // the travel speed of the projectile
    [Header("Missile Scaling Attributes")]
    public float projectileSpeedScalingFactor;

    AttackHitInfo hitInfo;


    // Use this for initialization
    protected override void Start() {
        base.Start(); // set weapon attributes

        this.parentEnemy = this.transform.parent.gameObject.GetComponent<Enemy>(); // set the parent enemy
        Vector3 vectorToPlayer = (this.target.transform.position - this.parentEnemy.transform.position).normalized * this.parentEnemy.attackRange; // get vector from enemy to player
        this.transform.localPosition = transform.localRotation * vectorToPlayer.normalized * 0.1f; // spawn the missile relative to the enemy and the player
        float angle = Mathf.Atan2(vectorToPlayer.x, vectorToPlayer.y) * Mathf.Rad2Deg * -1f; // get the angle toward the player
        this.transform.rotation = Quaternion.Euler(0, 0, angle); // update the missile's rotation
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed; // start the missile's travel
        hitInfo = new AttackHitInfo(damage, knockbackVelocity, knockbackDuration, parentEnemy.element, parentEnemy.elementalLevel, parentEnemy.gameObject);
        transform.parent = null;
    }

    protected override void OnTriggerStay2D(Collider2D col) {
        if ((!reflected && col.tag == "Player") || (reflected && col.tag == "Enemy"))
        {
            Actor actor = col.GetComponent<Actor>();
            Vector2 direction = transform.up;
            damage += (parentEnemy.attackScalingAmount * (GameManager.S.round - 1) / 2); // damage upgrade every two levels
            actor.Hit(hitInfo, direction);
            Destroy(gameObject);
        }
        else if (col.tag == "Wall" || col.tag == "Door")
        {
            Destroy(gameObject); // destroy on collision with player
        }
    }
}
