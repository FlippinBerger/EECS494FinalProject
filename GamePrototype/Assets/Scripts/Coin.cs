using UnityEngine;
using System.Collections;
using System;

public class Coin : Item {

    public override void OnPlayerPickup(Player p)
    {
        p.EnqueueFloatingText("+1 Gold", Color.yellow);
        GameManager.S.AddGold((int)value);
        Destroy(gameObject);
    }
}
