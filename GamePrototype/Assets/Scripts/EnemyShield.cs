using UnityEngine;
using System.Collections;

public class EnemyShield : Enemy {

	new public void Hit(AttackHitInfo hitInfo, Vector2 knockbackDirection) {
        /*
        if (knockbackDirection) { // if the knockback indicates that the direction came from in front of the shield

        }
        */

        base.Hit(hitInfo, knockbackDirection); // make the hit happen

        // make knockback happen even if damage was 0, as long as the enemy isn't currently invulnerable
        if (damage <= 0 && !knockedBack && !recoveringFromHit) {
            base.Knockback(knockbackVelocity, knockbackDirection, knockbackDuration);
        }
    }
}
