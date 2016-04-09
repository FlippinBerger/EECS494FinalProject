﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHeal : Weapon {
    private float maxSize = 4.0f;
    private float growthRate = 12.0f;
    private float scale = 1.0f;
    private float delayStart = 0.0f;
    public float delayTime = 2f;

    List<Player> healedPlayers = new List<Player>();

    // Use this for initialization
    protected override void Start()
    {
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.localPosition = new Vector3(0, 0);
    }
    void FixedUpdate()
    {
        // Handle the expansion of the heal bubble.
        if (scale < maxSize)
        {
            transform.localScale = Vector3.one * scale;
            scale += growthRate * Time.deltaTime;
        }
        else if (delayStart == 0.0f)
        {
            delayStart = Time.time;
        }
        else if (Time.time - delayStart > delayTime)
        {
            this.parentPlayer.StopDefense(this.cooldown);
            Destroy(gameObject);
        }
    }

    public override void Fire(float attackPower)
    {
        hitInfo = DetermineHitStrength(attackPower);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player pp = col.gameObject.GetComponent<Player>();
            if (healedPlayers.Contains(pp))
            {
                healedPlayers.Add(pp);
            }
            int healthBonus = pp.maxHealth/4;
            pp.currentHealth += healthBonus;
            pp.UpdateHealthBar();
        }
    }
}
