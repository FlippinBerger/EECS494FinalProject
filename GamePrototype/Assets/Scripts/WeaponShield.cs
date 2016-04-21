using UnityEngine;
using System.Collections;
using System;

public class WeaponShield : Weapon {

    public float shieldDuration = 1.0f;
    public float startTime = 0.0f;

    bool canReflect = false;

    protected override void UpgradeLevel2()
    {
        owner.EnqueueFloatingText("+ Shield size!", Color.green);
        Vector3 scale = transform.localScale;
        scale.x += 1;
        transform.localScale = scale;
    }

    protected override void UpgradeLevel3()
    {
        owner.EnqueueFloatingText("Shield reflects projectiles!", Color.green);
        canReflect = true;
    }

    protected override void UpgradeLevel4()
    {
        minDamage = 1;
        maxDamage = 1;
    }

    public override void Fire(float attackPower)
    {
        // hitInfo = DetermineHitStrength(attackPower);
        startTime = Time.time;
        gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Hazard")
        {
            Destroy(col.gameObject);
        }
        else if (col.tag == "Enemy")
        {
            Vector2 knockbackDirection = col.transform.position - owner.transform.position; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Hit(DetermineHitStrength(1f), knockbackDirection);
        }
        else if (col.tag == "EnemyWeapon")
        {
            EnemyWeaponMissile missile = col.gameObject.GetComponent<EnemyWeapon>() as EnemyWeaponMissile;
            if (missile != null && canReflect)
            {
                missile.GetComponent<Rigidbody2D>().velocity = transform.up * missile.projectileSpeed * 2;
                missile.transform.rotation = transform.rotation;
                missile.reflected = true;
                missile.gameObject.tag = "Weapon";
            }
            else
            {
                Destroy(col.gameObject);
            }
        }
    }
    
    void FixedUpdate()
    {
        this.transform.rotation = this.transform.parent.transform.rotation;
    }

    void Update()
    {
        // parentPlayer.Slow();
        if (Time.time - this.startTime > this.shieldDuration)
        {
            // parentPlayer.UnSlow();
            owner.StopDefense(this.cooldown); // stop attacking
        }
    }
}
