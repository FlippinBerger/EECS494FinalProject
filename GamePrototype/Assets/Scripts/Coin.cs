using UnityEngine;
using System.Collections;
using System;

public class Coin : Item {

    public override void OnPlayerPickup(Player p)
    {
        GameManager.S.AddGold(value);
        Destroy(gameObject);
    }
}
