﻿using UnityEngine;
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
        damagePerLevel = 1;
        base.UpgradeLevel3();
        // also get homing at this rank
    }

    public override void Fire(float attackPower) {
        this.numProjectiles = this.minProjectiles + (int)((this.maxProjectiles - this.minProjectiles) * attackPower);

        for (int i = 0; i < numProjectiles; i++) { // spawn numProjectiles (index starts at 1)
            float angle = (separationAngle/2) * (numProjectiles - 1) - (separationAngle * i);
            Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation * newRotation);
            ProjectileMagicMissile p = projectileGO.GetComponent<ProjectileMagicMissile>();
            p.SetHitInfo(DetermineHitStrength(attackPower));
            float range = maxRange;
            if (attackPower >= 1)
            {
                p.homing = true;
                range *= 2;
            }
            p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, range);
            p.currentRoom = owner.currentRoom;
        }

        owner.StopAttack();
    }

    void Update()
    {
        float charge = owner.currentAttackPower;
        float radius = chargeMinRadius + ((chargeMaxRadius - chargeMinRadius) * charge);
        transform.localScale = new Vector3(radius, radius, radius);
    }
}
