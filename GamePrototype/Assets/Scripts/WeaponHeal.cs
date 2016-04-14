using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHeal : Weapon {
    private float maxSize = 4.0f;
    private float growthRate = 12.0f;
    private float scale = 1.0f;
    private float delayStart = 0.0f;
    public float delayTime = 2f;

    bool fired = false;
    List<Player> healedPlayers = new List<Player>();

    public override void SetOwner(Player player)
    {
        base.SetOwner(player);
        transform.localPosition = Vector3.zero;
    }

    public override void Fire(float attackPower)
    {
        gameObject.SetActive(true);
        delayStart = Time.time;
        fired = true;
    }

    void Update()
    {
        if (!fired) return;

        // Handle the expansion of the heal bubble.
        if (scale < maxSize)
        {
            transform.localScale = Vector3.one * scale;
            scale += growthRate * Time.deltaTime;
        }
        else if (Time.time - delayStart > delayTime)
        {
            owner.StopDefense(this.cooldown);
        }
    }

    public override void ResetAttack()
    {
        base.ResetAttack();
        healedPlayers.Clear();
        transform.localScale = Vector3.one;
        scale = 1;
    }

    protected override void UpgradeLevel2()
    {
        maxSize += 2;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player pp = col.gameObject.GetComponent<Player>();
            if (!healedPlayers.Contains(pp))
            {
                healedPlayers.Add(pp);
                pp.currentHealth += maxDamage;
                pp.UpdateHealthBar();
                if (GetUpgradeLevel() > 2)
                {
                    pp.Cleanse();
                }
            }
        }
    }
}
