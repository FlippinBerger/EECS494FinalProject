using UnityEngine;
using System.Collections;

public class WeaponMagicHands : WeaponRanged {
    public int minProjectiles = 1;
    public int maxProjectiles = 3;
    public float separationAngle = 3f; // the angle of separation between each projectile
    public float chargeMinRadius = 0.2f;
    public float chargeMaxRadius = 0.5f;

    private int numProjectiles;

    public override void ResetAttack()
    {
        base.ResetAttack();
        transform.localScale = new Vector3(chargeMinRadius, chargeMinRadius, chargeMinRadius);
    }

    protected override void UpgradeLevel2()
    {
        ++maxProjectiles;
        chargeTime -= 0.25f;
    }

    protected override void UpgradeLevel3()
    {
        UpgradeLevel2();
    }

    // upgrade 4 is homing

    protected override void UpgradePast4()
    {
        minDamage += 1;
        maxDamage += 1;
    }

    public override void Fire(float attackPower) {
        this.numProjectiles = this.minProjectiles + (int)((this.maxProjectiles - this.minProjectiles) * attackPower);

        for (int i = 0; i < numProjectiles; i++) { // spawn numProjectiles (index starts at 1)
            float angle = (separationAngle/2) * (numProjectiles - 1) - (separationAngle * i);
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation * newRotation);
            ProjectileMagicMissile p = projectileGO.GetComponent<ProjectileMagicMissile>();
            p.SetHitInfo(DetermineHitStrength(attackPower));
            if (upgradeLevel > 3 && attackPower >= 1)
            {
                p.homing = true;
            }
            p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);
            p.currentRoom = parentPlayer.currentRoom;
        }

        parentPlayer.StopAttack();
    }

    void Update()
    {
        float charge = parentPlayer.currentAttackPower;
        float radius = chargeMinRadius + ((chargeMaxRadius - chargeMinRadius) * charge);
        transform.localScale = new Vector3(radius, radius, radius);
    }
}
