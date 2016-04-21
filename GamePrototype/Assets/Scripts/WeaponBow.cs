using UnityEngine;
using System.Collections;

public class WeaponBow : WeaponRanged {
    
    protected LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void ResetAttack()
    {
        base.ResetAttack();
        lineRenderer.enabled = false;
    }

    protected override void UpgradeLevel2()
    {
        owner.EnqueueFloatingText("Attacks pierce when fully charged!", Color.green);
        base.UpgradeLevel2();
        chargeTime /= 2;
        maxRange += 5;
    }

    protected override void UpgradeLevel3()
    {
        base.UpgradeLevel3();
        owner.EnqueueFloatingText("Attacks slow!", Color.green);
    }

    protected override void UpgradeLevel4()
    {
        base.UpgradeLevel4();
        owner.EnqueueFloatingText("Attacks can crit!", Color.green);
    }

    public override void Fire(float attackPower)
    {
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, transform.position, transform.rotation);
        ProjectileArrow p = projectileGO.GetComponent<ProjectileArrow>();
        p.SetHitInfo(DetermineHitStrength(attackPower));
        p.SetMissileInfo(attackPower, minMissileSpeed, maxMissileSpeed, minRange, maxRange);
        int upgradeLevel = GetUpgradeLevel();
        p.canPierce = (upgradeLevel > 1);
        p.canSlow = (upgradeLevel > 2);
        p.canCrit = (upgradeLevel > 3);
        owner.StopAttack();
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
        DrawRangeIndicator(owner.currentAttackPower);
    }
}
