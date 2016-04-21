using UnityEngine;
using System.Collections;
using System;

public class ManaPotion : Item {

    public override void OnPlayerPickup(Player p)
    {
        p.AddMana((int)value);
        Destroy(gameObject);
    }
}
