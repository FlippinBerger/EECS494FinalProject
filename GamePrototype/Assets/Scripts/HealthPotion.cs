using UnityEngine;
using System.Collections;
using System;

public class HealthPotion : Item {

    public override void OnPlayerPickup(Player p)
    {
        p.currentHealth += (int)value;
        p.UpdateHealthBar();
        p.EnqueueFloatingText(value + " Health restored!", Color.green);
        Destroy(gameObject);
    }
}
