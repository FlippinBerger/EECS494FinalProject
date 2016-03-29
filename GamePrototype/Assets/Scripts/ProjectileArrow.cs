using UnityEngine;
using System.Collections;

public class ProjectileArrow : Projectile {

	// Use this for initialization
	void Start () {
	
	}

    public override void SetMissileInfo(float attackPower, float minMissileSpeed, float maxMissileSpeed, float minRange, float maxRange) {
        base.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);

        if (this.supercharged) { // if fully charged
            this.range = 30f; // set range to the length of the entire room
        }
    }

    public override void HandleEnemyCollision(Collider2D col) {
        // handle all previous concerns
        Vector2 knockbackDirection = GetComponent<Rigidbody2D>().velocity; // calculate knockback direction
        knockbackDirection.Normalize(); // make knockbackDirection a unit vector
        col.gameObject.GetComponent<Enemy>().Hit(hitInfo, knockbackDirection); // deal damage to the enemy
        if (!supercharged) {
            Destroy(this.gameObject); // destroy the projectile if not fully charged
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
