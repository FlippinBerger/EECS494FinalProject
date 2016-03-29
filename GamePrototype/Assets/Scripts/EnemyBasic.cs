using UnityEngine;
using System.Collections;

public class EnemyBasic : Enemy {

    public GameObject enemyWeaponPrefab;

    protected override void Attack(GameObject target) {
        base.Attack(target); // provide base functions

        GameObject enemyWeaponGO = Instantiate(this.enemyWeaponPrefab); // instantiate the weapon
        enemyWeaponGO.transform.parent = this.transform; // set the parent transform
        EnemyWeapon enemyWeapon = enemyWeaponGO.GetComponent<EnemyWeapon>(); // keep track of the weapon's variables

        // set the weapon's attributes
        enemyWeapon.damage = this.damage;
        enemyWeapon.knockbackDuration = this.knockbackDuration;
        enemyWeapon.knockbackVelocity = this.knockbackVelocity;
        enemyWeapon.target = target;
    }
}
