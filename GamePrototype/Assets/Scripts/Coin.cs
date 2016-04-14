using UnityEngine;
using System.Collections;
using System;

public class Coin : Item {

    public override void OnPlayerPickup(Player p)
    {
        p.AddGold(value);
        Destroy(gameObject);
    }
}
