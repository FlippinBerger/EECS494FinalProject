﻿using UnityEngine;
using System.Collections;

public class WeaponShield : Weapon {

    public float shieldDuration = 1.0f;
    public float startTime = 0.0f;

    // Use this for initialization
    void Start()
    {
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.localPosition =  new Vector3(0, .8f);
        this.startTime = Time.time;
        this.transform.rotation = this.transform.parent.transform.rotation;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
       
        //If an enemy or a projectile hits the shield, knock them back
        //We don't have any projectiles yet, so need to work out what would classify.
        if (col.gameObject.tag == "Enemy")
        {
            Vector2 knockbackDirection = col.transform.position - this.parentPlayer.transform.position; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Knockback(this.knockbackVelocity, knockbackDirection, this.knockbackDuration);
        }

        if (col.gameObject.tag == "Hazard")
        {
            Destroy(col.gameObject);
        }
    }
    
    void FixedUpdate()
    {
        this.transform.rotation = this.transform.parent.transform.rotation;
    }

    void Update()
    {
        if (Time.time - this.startTime > this.shieldDuration)
        { // if the sword has completed its arc
            this.parentPlayer.StopDefense(this.cooldown); // stop attacking
            Destroy(this.gameObject); // destroy the sword object
        }
    }
}
