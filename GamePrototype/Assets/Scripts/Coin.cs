﻿using UnityEngine;
using System.Collections;
using System;

public class Coin : Item {

    protected override void OnPlayerPickup(Player p)
    {
        p.AddGold(value);
        Destroy(gameObject);
    }
}
