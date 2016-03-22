using UnityEngine;
using System.Collections;

public class EnemyElemental : Enemy {

    protected override void Burn(int damage) {
        base.Burn(damage * -1); // heal from burn damage
    }
}
