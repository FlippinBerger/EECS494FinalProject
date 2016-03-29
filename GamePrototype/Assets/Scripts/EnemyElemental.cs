using UnityEngine;
using System.Collections;

public class EnemyElemental : Enemy {

    protected override void Burn(int damage) {
        base.Burn(0); // take no damage from burn
        Enrage();
    }

    
}
