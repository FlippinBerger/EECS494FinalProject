using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    protected AttackHitInfo hitInfo;
    protected float missileSpeed;
    protected float range;
    protected float distTraveled = 0;
    protected bool supercharged = false;
    
    public void SetHitInfo(AttackHitInfo hitInfo)
    {
        this.hitInfo = hitInfo;
    }

    public void SetMissileInfo(float attackPower, float minMissileSpeed, float maxMissileSpeed, float minRange, float maxRange)
    {
        missileSpeed = minMissileSpeed + ((maxMissileSpeed - minMissileSpeed) * attackPower);
        range = minRange + ((maxRange - minRange) * attackPower);
        if (attackPower >= 1) supercharged = true;
        GetComponent<Rigidbody2D>().velocity = transform.up * missileSpeed;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") { // if the weapon hits an enemy
            Vector2 knockbackDirection = GetComponent<Rigidbody2D>().velocity; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Hit(hitInfo, knockbackDirection); // deal damage to the enemy
            Destroy(this.gameObject);
        }
        else if (col.tag == "Wall")
        {
            Destroy(this.gameObject); // destroy on collision with wall
        }
    }

    void Update() {
        distTraveled += missileSpeed * Time.deltaTime;
        if (distTraveled > range)
        {
            Destroy(this.gameObject);
        }
    }
}
