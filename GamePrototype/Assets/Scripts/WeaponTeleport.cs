using UnityEngine;
using System.Collections;

public class WeaponTeleport : Weapon {
    protected override void Start()
    {
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.position = this.parentPlayer.transform.position;
        this.parentPlayer.transform.position += (this.parentPlayer.transform.rotation)  * (Vector3.one * 2.0f);
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
        this.parentPlayer.StopDefense(this.cooldown);
        Destroy(gameObject, exp.duration);
    }

    public override void Fire(float attackPower)
    {
        hitInfo = DetermineHitStrength(attackPower);
    }
}
