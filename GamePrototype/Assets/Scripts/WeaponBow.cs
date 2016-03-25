using UnityEngine;
using System.Collections;
using System;

public class WeaponBow : Weapon {

    public override void Fire(float attackPower)
    {
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
    }
}
