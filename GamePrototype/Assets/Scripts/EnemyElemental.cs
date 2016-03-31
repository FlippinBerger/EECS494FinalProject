using UnityEngine;
using System.Collections;

public class EnemyElemental : Enemy {

    public Element element;

    protected override void Start()
    {
        // find out what biome we're in from dungeonlayoutgenerator, set element and color
        base.Start();
    }

    public override void Burn(int damage) {
        if (element == Element.Fire)
        {
            damage = 0; // take no damage from burn
            Enrage();
        }
        base.Burn(damage);
    }

    public override void Freeze(float freezeStrength)
    {
        if (element == Element.Ice)
        {
            freezeStrength = 0;
            Enrage();
        }
        base.Freeze(freezeStrength);
    }

}
