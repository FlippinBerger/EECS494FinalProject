using UnityEngine;
using System.Collections;
using System;

public class EnemyWeaponFist : EnemyWeapon {

    public float swingAngle = 120.0f; // the angle of the swing arc
    public float swingSpeed = 5f; // minimum speed of the sword swing

    private float weaponRotationAngle = 0f; // the current rotation of the fist in degrees relative to the enemy

    // Use this for initialization
    void Start() {
        this.weaponRotationAngle = -1 * (this.swingAngle / 2f); // set the starting angle for the enemy
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.weaponRotationAngle)); // update the fist's rotation
        this.parentEnemy = this.transform.parent.gameObject.GetComponent<Enemy>(); // set the parent enemy
        Vector3 vectorToPlayer = (this.target.transform.position - this.parentEnemy.transform.position).normalized * this.parentEnemy.attackRange; // get vector from enemy to player
        this.transform.localPosition = transform.localRotation * vectorToPlayer; // spawn the fist relative to the enemy and the player
    }

    void Update() {
            // update the fist's position
            this.weaponRotationAngle += swingSpeed * 100f * Time.deltaTime; // find the fist's new rotation based on swing speed
            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.weaponRotationAngle)); // update the fist's rotation
            Vector3 pos = new Vector3(0, 1f, 0); // get its distance from the center of the enemy
            Vector3 vectorToPlayer = (this.target.transform.position - this.parentEnemy.transform.position).normalized * this.parentEnemy.attackRange; // get vector from enemy to player
            pos = this.transform.localRotation * vectorToPlayer; // rotate the fist around the enemy
            this.transform.localPosition = pos; // set the fist's position

            if (this.weaponRotationAngle >= this.swingAngle / 2f) { // if the fist has completed its arc
                Destroy(this.gameObject); // destroy the fist object
        }
    }
}
