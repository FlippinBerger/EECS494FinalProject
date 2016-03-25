using UnityEngine;
using System.Collections;

public class WeaponMagicHands : Weapon {

    override public void Fire(float attackPower)
    {
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
        // do stuff with attackpower
    }
}
