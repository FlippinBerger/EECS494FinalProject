using UnityEngine;
using System.Collections;
using System;

public class IceOrb : Item {

    public override void OnPlayerPickup(Player p)
    {
        p.EnqueueFloatingText("Ice level increased!", Color.cyan);
        if (p.elementalLevel == 1)
        {
            p.EnqueueFloatingText("Attacks freeze enemies", Color.cyan);
            p.EnqueueFloatingText("and damage fire hazards", Color.cyan);
        }
        p.EnqueueFloatingText("+ Max Health", Color.cyan);
        p.UpgradeElementalLevel(Element.Ice);
    }
}
