using UnityEngine;
using System.Collections;

public class WeaponBlast : Weapon {
    private float maxSize = 4.0f;
    private float growthRate = 12.0f;
    private float scale = 1.0f;
    private float delayStart = 0.0f;
    public float delayTime = 0.0f;
    public float knockPower = 1.0f;
    public float knockDur = 1.0f;

    // Use this for initialization
    // TODO change this to SetOwner
    /*
    protected override void Start()
    {
        this.owner = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.localPosition = new Vector3(0, 0);
    }
    */
    void FixedUpdate()
    {
        // Handle the expansion of the heal bubble.
        if (scale < maxSize)
        {
            transform.localScale = Vector3.one * scale;
            scale += growthRate * Time.deltaTime;
        }
        else if (delayStart == 0.0f)
        {
            delayStart = Time.time;
        }
        else if (Time.time - delayStart > delayTime)
        {
            this.owner.StopDefense(this.cooldown);
            Destroy(gameObject);
        }
    }

    public override void Fire(float attackPower)
    {
        hitInfo = DetermineHitStrength(attackPower);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        //If an enemy or a projectile hits the shield, knock them back
        //We don't have any projectiles yet, so need to work out what would classify.
        if (col.gameObject.tag == "Enemy")
        {
            Vector2 knockbackDirection = col.transform.position - this.owner.transform.position; // calculate knockback direction
            knockbackDirection.Normalize(); // make knockbackDirection a unit vector
            col.gameObject.GetComponent<Enemy>().Knockback(knockPower, knockbackDirection,knockDur); // deal damage to the enemy
        }
    }
}
