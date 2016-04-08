using UnityEngine;
using System.Collections;
using System;

public class WeaponRanged : Weapon {

    public float minMissileSpeed = 8f;
    public float maxMissileSpeed = 18f;
    public float minRange = 3f;
    public float maxRange = 10f;

    protected LineRenderer lineRenderer;

    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void Fire(float attackPower)
    {
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile p = projectileGO.GetComponent<Projectile>();
        p.SetHitInfo(DetermineHitStrength(attackPower));
        p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);
        parentPlayer.StopAttack();
    }

    protected virtual void DrawRangeIndicator(float attackPower)
    {
        float range = minRange + ((maxRange - minRange) * attackPower);
        lineRenderer.SetPositions(new Vector3[] { transform.position, transform.position + transform.up * range });
        lineRenderer.sortingOrder = 900;
        lineRenderer.enabled = true;
    }

    protected virtual void Update()
    {
        DrawRangeIndicator(parentPlayer.currentAttackPower);
    }
}
