using UnityEngine;
using System.Collections;

public class FireOrb : Item {
    
    public override void OnPlayerPickup(Player p)
    {
        p.EnqueueFloatingText("Fire level increased!", Color.red);
        if (p.elementalLevel == 1)
        {
            p.EnqueueFloatingText("Attacks burn enemies", Color.red);
            p.EnqueueFloatingText("and damage ice hazards", Color.red);
        }
        p.EnqueueFloatingText("+ Attack Speed", Color.red);
        p.UpgradeElementalLevel(Element.Fire);
    }
}
