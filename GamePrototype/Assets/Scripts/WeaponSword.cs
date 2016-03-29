using UnityEngine;
using System.Collections;
using System;

public class WeaponSword : Weapon {

    public float minSwingAngle = 120.0f; // the minimum angle of the swing arc
    public float maxSwingAngle = 360.0f; // the maximum angle of the swing arc
    public float minSwingSpeed = 4f; // minimum speed of the sword swing
    public float maxSwingSpeed = 10f; // maximum speed of the sword swing

    // protected AttackHitInfo hitInfo;

    private float swordRotationAngle = 0f; // the current rotation of the sword in degrees relative to the player
    private float swingAngle;
    private float swingSpeed;
    bool swing = false;

    // Use this for initialization
    void Start () {
        this.swordRotationAngle = -1 * (this.minSwingAngle / 2f); // set the starting angle for the sword
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.swordRotationAngle)); // update the sword's rotation
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.localPosition = transform.localRotation * new Vector3(0, 1f, 0); // spawn the sword relative to the player
    }

    public override void Fire(float attackPower)
    {
        // modify damage, knockback, swing angle, etc.
        hitInfo = DetermineHitStrength(attackPower);
        swing = true;
    }

    override protected AttackHitInfo DetermineHitStrength(float attackPower) {
        // modify swing speed and angle
        this.swingSpeed = this.minSwingSpeed + ((this.maxSwingSpeed - this.minSwingSpeed) * attackPower);
        this.swingAngle = this.minSwingAngle + ((this.maxSwingAngle - this.minSwingAngle) * attackPower);

        return base.DetermineHitStrength(attackPower); // return AttackHitInfo

    }

    public void OnTriggerStay2D(Collider2D col) {
        if (swing && col.gameObject.tag == "Enemy") { // if the sword hits an enemy
            Vector2 knockbackDirection = col.transform.position - this.parentPlayer.transform.position; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Hit(hitInfo, knockbackDirection); // deal damage to the enemy
        }
    }
	
	void Update () {
        // if the sword hasn't been swung
        if (!swing) {
            // set the windup angle
            float attackPower = this.parentPlayer.currentAttackPower; // get current attack power
            DetermineHitStrength(attackPower); // set swing speed and angle
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.swordRotationAngle)); // update the sword's rotation
            this.swordRotationAngle = -1 * (this.swingAngle / 2f); // set the angle for the windup
            Vector3 pos = new Vector3(0, 1f, 0); // get its distance from the center of the player
            pos = this.transform.localRotation * pos; // rotate the sword around the player
            this.transform.localPosition = pos; // set the sword's position
        }
        // after the sword begins swinging
        else {
            // update the sword's position
            this.swordRotationAngle += swingSpeed * 100f * Time.deltaTime; // find the sword's new rotation based on swing speed
            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.swordRotationAngle)); // update the sword's rotation
            Vector3 pos = new Vector3(0, 1f, 0); // get its distance from the center of the player
            pos = this.transform.localRotation * pos; // rotate the sword around the player
            this.transform.localPosition = pos; // set the sword's position

            if (this.swordRotationAngle >= this.swingAngle / 2f) { // if the sword has completed its arc
                this.parentPlayer.StopAttack(); // stop attacking
                                                // Destroy(this.gameObject); // destroy the sword object
            }
        }
    }
}
