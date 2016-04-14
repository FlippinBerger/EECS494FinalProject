using UnityEngine;
using System.Collections;

public class WeaponTeleport : Weapon {
    // TODO change this to SetOwner()
    /*
    protected override void Start()
    {
        this.owner = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.position = this.owner.transform.position;
        this.owner.transform.position += (this.owner.transform.rotation)  * (Vector3.one * 2.0f);
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
        this.owner.StopDefense(this.cooldown);
        Destroy(gameObject, exp.duration);
    }
    */

    public override void Fire(float attackPower)
    {
        hitInfo = DetermineHitStrength(attackPower);
    }
}
