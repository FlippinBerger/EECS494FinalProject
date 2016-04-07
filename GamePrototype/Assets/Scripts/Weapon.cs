﻿using UnityEngine;
using System.Collections;

public struct AttackHitInfo
{
    public AttackHitInfo(int d, float kV, float kD)
    {
        damage = d;
        knockbackVelocity = kV;
        knockbackDuration = kD;
    }
    public int damage;
    public float knockbackVelocity;
    public float knockbackDuration;
}

public abstract class Weapon : MonoBehaviour {
    public string weaponName;
    public bool isSpell = false; // isSpell determines whether the weapon gets bound
                                 // to the right trigger or the left trigger
                                 // (defensive abilities are called "spells" for now)
    public int manaCost = 0;
    public int minDamage = 1;
    public int maxDamage = 3;
    public float minKnockbackVelocity = 3f;
    public float maxKnockbackVelocity = 5f;
    public float minKnockbackDuration = 0.1f;
    public float maxKnockbackDuration = 0.5f;
    public float cooldown = 1f; // the cooldown between attacks
    public float chargeTime = 1f;
    public Sprite icon; // the icon that represents this weapon
    public GameObject projectilePrefab; // null for melee weapons

    protected Player parentPlayer; // the player associated with this weapon
    protected AttackHitInfo hitInfo;
    // protected float attackPower; // value from [0-1] depending on how charged this attack was
    
    // Use this for initialization
    protected virtual void Start () {
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
    }

    abstract public void Fire(float attackPower);

    virtual protected AttackHitInfo DetermineHitStrength(float attackPower)
    {
        int damage = minDamage + (int)((maxDamage - minDamage) * attackPower);
        float knockbackVelocity = minKnockbackVelocity + ((maxKnockbackVelocity - minKnockbackVelocity) * attackPower);
        float knockbackDuration = minKnockbackDuration + ((maxKnockbackDuration - minKnockbackDuration) * attackPower);
        return new AttackHitInfo(damage, knockbackVelocity, knockbackDuration);
    }
}
