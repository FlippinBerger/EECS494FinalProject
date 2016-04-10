using UnityEngine;
using System.Collections;
using System;

public abstract class WeaponRanged : Weapon {

    public GameObject projectilePrefab;
    public float minMissileSpeed = 8f;
    public float maxMissileSpeed = 18f;
    public float minRange = 3f;
    public float maxRange = 10f;

}
