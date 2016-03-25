using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {
    public int damage = 1;  // the amount of damage the weapon does
    public float cooldown = 1f; // the cooldown between attacks
    public float knockbackVelocity = 3.0f; // the velocity with which this weapon knocks enemies backward
    public float knockbackDuration = 0.1f; // the amount of time this weapon knocks enemies backward
    public float chargeTime = 1f;
    public Sprite icon; // the icon that represents this weapon
    public GameObject projectilePrefab; // null for melee weapons

    protected Player parentPlayer; // the player associated with this weapon
    protected float attackPower; // value from [0-1] depending on how charged this attack was

    /*
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    */

    abstract public void Fire(float attackPower);

    /*
    public void SetChargePower(float power)
    {
        if (power > 1) power = 1;
        if (power < 0) power = 0;
        attackPower = power;
    }
    */
}
