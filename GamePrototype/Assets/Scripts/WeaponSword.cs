using UnityEngine;
using System.Collections;
using System;

public class WeaponSword : Weapon {

    public Vector2 hitboxDimensions;
    public float minSwingAngle = 120.0f; // the minimum angle of the swing arc
    public float maxSwingAngle = 360.0f; // the maximum angle of the swing arc
    public float minSwingSpeed = 4f; // minimum speed of the sword swing
    public float maxSwingSpeed = 10f; // maximum speed of the sword swing
    public float distFromPlayer = 1.2f;
    public float berserkDuration = 2f;

    float berserkStartTime;
    bool berserkMode = false;
    TrailRenderer trailRenderer;

    // protected AttackHitInfo hitInfo;

    private float swordRotationAngle = 0f; // the current rotation of the sword in degrees relative to the player
    private float swingAngle;
    private float swingSpeed;
    bool swing = false;
    BoxCollider2D hitbox;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;
        hitbox = gameObject.AddComponent<BoxCollider2D>();
        hitbox.size = hitboxDimensions;
        hitbox.isTrigger = true;
        hitbox.enabled = false;
    }

    public override void ResetAttack()
    {
        base.ResetAttack();
        hitbox.enabled = false;
        this.swordRotationAngle = -1 * (this.minSwingAngle / 2f); // set the starting angle for the sword
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.swordRotationAngle)); // update the sword's rotation
        this.transform.localPosition = transform.localRotation * new Vector3(0, distFromPlayer, 0); // spawn the sword relative to the player
        swing = false;
        trailRenderer.enabled = false;
    }

    protected override void UpgradeLevel2()
    {
        base.UpgradeLevel2();
        Vector3 scale = transform.localScale;
        scale.y += 0.5f;
        transform.localScale = scale;
        distFromPlayer += 0.25f;
    }

    protected override void UpgradeLevel3()
    {
        base.UpgradeLevel3();
        chargeTime /= 2;
    }

    public override void Fire(float attackPower)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        // modify damage, knockback, swing angle, etc.
        hitInfo = DetermineHitStrength(attackPower);
        // attach hitbox
        hitbox.enabled = true;
        
        trailRenderer.enabled = true;
        berserkMode = (attackPower >= 1) && (GetUpgradeLevel() > 2);
        if (berserkMode)
        {
            berserkStartTime = Time.time;
            owner.canRotate = false;
        }
        swing = true;
    }

    override protected AttackHitInfo DetermineHitStrength(float attackPower) {
        // modify swing speed and angle
        this.swingSpeed = this.minSwingSpeed + ((this.maxSwingSpeed - this.minSwingSpeed) * attackPower);
        this.swingAngle = this.minSwingAngle + ((this.maxSwingAngle - this.minSwingAngle) * attackPower);

        return base.DetermineHitStrength(attackPower); // return AttackHitInfo

    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (!swing) return;
        if (col.tag == "Enemy") { // if the sword hits an enemy
            Vector2 knockbackDirection = col.transform.position - this.owner.transform.position; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Hit(hitInfo, knockbackDirection); // deal damage to the enemy
        }
        else if (col.tag == "EnemyWeapon" && berserkMode)
        {
            Destroy(col.gameObject);
        }
    }
	
	void Update () {
        // if the sword hasn't been swung
        if (!swing) {
            // set the windup angle
            float attackPower = this.owner.currentAttackPower; // get current attack power
            DetermineHitStrength(attackPower); // set swing speed and angle
            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.swordRotationAngle)); // update the sword's rotation
            this.swordRotationAngle = -1 * (this.swingAngle / 2f); // set the angle for the windup
            Vector3 pos = new Vector3(0, distFromPlayer, 0); // get its distance from the center of the player
            pos = this.transform.localRotation * pos; // rotate the sword around the player
            this.transform.localPosition = pos; // set the sword's position
        }
        // after the sword begins swinging
        else {
            // update the sword's position
            this.swordRotationAngle += swingSpeed * 100f * Time.deltaTime; // find the sword's new rotation based on swing speed
            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.swordRotationAngle)); // update the sword's rotation
            Vector3 pos = new Vector3(0, distFromPlayer, 0); // get its distance from the center of the player
            pos = this.transform.localRotation * pos; // rotate the sword around the player
            this.transform.localPosition = pos; // set the sword's position

            if ((berserkMode && Time.time - berserkStartTime > berserkDuration) ||
                (!berserkMode && this.swordRotationAngle >= this.swingAngle / 2f))
            {
                owner.canRotate = true;
                this.owner.StopAttack(); // stop attacking
            }
        }
    }
}
