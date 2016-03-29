using UnityEngine;
using System.Collections;

public class WeaponMagicHands : WeaponRanged {
    public int minProjectiles = 1;
    public int maxProjectiles = 5;
    public float separationAngle = 3f; // the angle of separation between each projectile

    private int numProjectiles;

    public override void Fire(float attackPower) {
        this.numProjectiles = this.minProjectiles + (int)((this.maxProjectiles - this.minProjectiles) * attackPower);
        // calculate middle projectile
        /*
        int middleProjectile = this.numProjectiles / 2; // initial calculation
        if (this.numProjectiles % 2 != 1) { // if the number of projectiles is odd
            middleProjectile += 1; // add one to the middle projectile calculation
        }
        */

        for (int i = 1; i <= numProjectiles; i++) { // spawn numProjectiles (index starts at 1)
            // determine the rotation for the projectile
            // int index = i - middleProjectile;
            float angle = (separationAngle/2) * (numProjectiles - 1) - (separationAngle * (i-1));
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation * newRotation);
            Projectile p = projectileGO.GetComponent<Projectile>();
            p.SetHitInfo(DetermineHitStrength(attackPower));
            p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);
        }
    }
}
