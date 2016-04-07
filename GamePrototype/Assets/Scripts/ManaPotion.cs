using UnityEngine;
using System.Collections;
using System;

public class ManaPotion : Item {

    protected override void OnPlayerPickup(Player p)
    {
        p.AddMana(value);
        Destroy(gameObject);
    }
}
