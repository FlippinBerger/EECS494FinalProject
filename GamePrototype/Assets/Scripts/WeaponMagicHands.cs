using UnityEngine;
using System.Collections;

public class WeaponMagicHands : WeaponRanged {
    public int minProjectiles = 1;
    public int maxProjectiles = 5;
    public float separationAngle = 3f; // the angle of separation between each projectile
    public float chargeMinRadius = 0.2f;
    public float chargeMaxRadius = 0.5f;

    private int numProjectiles;

    public override void ResetAttack()
    {
        base.ResetAttack();
        transform.localScale = new Vector3(chargeMinRadius, chargeMinRadius, chargeMinRadius);
    }

    public override void Fire(float attackPower) {
        this.numProjectiles = this.minProjectiles + (int)((this.maxProjectiles - this.minProjectiles) * attackPower);

        for (int i = 0; i < numProjectiles; i++) { // spawn numProjectiles (index starts at 1)
            float angle = (separationAngle/2) * (numProjectiles - 1) - (separationAngle * i);
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation * newRotation);
            Projectile p = projectileGO.GetComponent<Projectile>();
            p.SetHitInfo(DetermineHitStrength(attackPower));
            p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);
        }

        parentPlayer.StopAttack();
    }

    protected override void Update()
    {
        float charge = parentPlayer.currentAttackPower;
        float radius = chargeMinRadius + ((chargeMaxRadius - chargeMinRadius) * charge);
        transform.localScale = new Vector3(radius, radius, radius);
    }
}
