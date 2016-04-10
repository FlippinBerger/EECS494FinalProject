using UnityEngine;
using System.Collections;

public class ProjectileArrow : Projectile {

    // upgrades
    [HideInInspector]
    public bool canPierce = false;
    [HideInInspector]
    public bool canSlow = false;
    [HideInInspector]
    public bool canCrit = false;

    public override void SetMissileInfo(float attackPower, float minMissileSpeed, float maxMissileSpeed, float minRange, float maxRange) {
        base.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);

        if (this.supercharged) { // if fully charged
            this.range = 30f; // set range to the length of the entire room
        }
    }

    public override void HandleEnemyCollision(Collider2D col) {
        Enemy enemy = col.GetComponent<Enemy>();
        bool pierceFlag = false;
        if (supercharged)
        {
            if (canPierce)
            {
                pierceFlag = true;
            }
            if (canSlow)
            {
                enemy.Slow(); // currently we don't unslow them
                enemy.EnqueueFloatingText("Maimed!", Color.red);
            }
            if (canCrit)
            {
                int roll = Random.Range(0, 5);
                if (roll == 0)
                {
                    hitInfo.damage *= 2;
                    enemy.EnqueueFloatingText("CRIT!", Color.red);
                }
            }
        }
        
        Vector2 knockbackDirection = GetComponent<Rigidbody2D>().velocity; // calculate knockback direction
        knockbackDirection.Normalize(); // make knockbackDirection a unit vector
        col.gameObject.GetComponent<Enemy>().Hit(hitInfo, knockbackDirection); // deal damage to the enemy

        if (!pierceFlag) {
            Destroy(this.gameObject); // destroy the projectile if not piercing
        }
    }
}
