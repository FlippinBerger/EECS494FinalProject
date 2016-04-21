using UnityEngine;
using System.Collections;
using System;

public class IceOrb : Item {

    public override void OnPlayerPickup(Player p)
    {
        p.EnqueueFloatingText("Ice level increased!", Color.cyan);
        /*
        if (p.elementalLevel == 1)
        {
            p.EnqueueFloatingText("Attacks freeze enemies", Color.cyan);
            p.EnqueueFloatingText("and damage fire hazards", Color.cyan);
        }
        */
        p.EnqueueFloatingText("+" + (int)value + " Max Health", Color.cyan);
        p.currentHealth += (int)value;
        p.maxHealth += (int)value;
        p.UpdateHealthBar();
        p.UpgradeElementalLevel(Element.Ice);
    }
}
