using UnityEngine;
using System.Collections;
using System;

public class WeaponRanged : Weapon {

    public float minMissileSpeed = 8f;
    public float maxMissileSpeed = 18f;
    public float minRange = 3f;
    public float maxRange = 10f;

    public override void Fire(float attackPower)
    {
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile p = projectileGO.GetComponent<Projectile>();
        p.SetHitInfo(DetermineHitStrength(attackPower));
        p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);
    }
}
