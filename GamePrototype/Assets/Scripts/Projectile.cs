using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    protected AttackHitInfo hitInfo;
    protected float missileSpeed;
    protected float range;
    protected float distTraveled = 0;
    protected bool supercharged = false;
    protected Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    
    public void SetHitInfo(AttackHitInfo hitInfo)
    {
        this.hitInfo = hitInfo;
    }

    public virtual void SetMissileInfo(float attackPower, float minMissileSpeed, float maxMissileSpeed, float minRange, float maxRange)
    {
        missileSpeed = minMissileSpeed + ((maxMissileSpeed - minMissileSpeed) * attackPower);
        range = minRange + ((maxRange - minRange) * attackPower);
        if (attackPower >= 1) supercharged = true;
        rigid.velocity = transform.up * missileSpeed;
    }

    public virtual void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") { // if the weapon hits an enemy
            HandleEnemyCollision(col);
        }
        else if (col.tag == "Wall" || col.tag == "Door")
        {
            Destroy(this.gameObject); // destroy on collision with wall
        }
    }
    public virtual void HandleEnemyCollision(Collider2D col) {
        Vector2 knockbackDirection = GetComponent<Rigidbody2D>().velocity; // calculate knockback direction
        knockbackDirection.Normalize(); // make knockbackDirection a unit vector
        col.gameObject.GetComponent<Enemy>().Hit(hitInfo, knockbackDirection); // deal damage to the enemy
        Destroy(this.gameObject);
    }
    protected virtual void Update() {
        distTraveled += missileSpeed * Time.deltaTime;
        if (distTraveled > range)
        {
            Destroy(this.gameObject);
        }
    }
}
