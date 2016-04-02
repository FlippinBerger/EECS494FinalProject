﻿using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
    public int maxHealth; // the amount of damage the actor can take before dying
    public int currentHealth;
    public float hitRecoveryTime; // the time the actor spends invulnerable after being hit
    public float hitFlashInterval; // the amount of time in between color flashes when hit
    public Color flashColor = Color.red; // the color the sprite will flash when hit
    public float moveSpeed; // the movement speed of this enemy
    public float healthBarFadeOutTime = 1f;
    public int numBurnTicks = 3;
    public float burnTickInterval = 2f;
    public float freezeDuration = 5f;
    public float freezeDecay = 0.5f;
    public float slowFactor = 0.33f;

    protected float recoveryTimeElapsed = 0.0f; // the time elapsed since hit
    protected bool knockedBack = false; // whether the enemy is currently knocked back or not
    protected bool recoveringFromHit = false; // whether the enemy is recovering or not
    protected bool burning = false;
    protected bool frozen = false;
    protected bool slowed = false;
    Coroutine burntickCoroutine;
    protected float freezePoints = 0;
    protected float frozenStartTime;
    protected Color originalSpriteColor; // the default color of the sprite

    protected GameObject canvases;
    GameObject healthBarCanvas;
    GameObject statusEffectCanvas;
    float healthBarFadeStart;

    // Use this for initialization
    protected virtual void Start () {
        this.originalSpriteColor = this.GetComponent<SpriteRenderer>().color; // keep track of the sprite's original color
        this.recoveryTimeElapsed = this.hitRecoveryTime; // don't start by being invulnerable
        currentHealth = maxHealth;
        canvases = transform.FindChild("Actor Canvases").gameObject;
        healthBarCanvas = canvases.transform.FindChild("Health Bar").gameObject;
        statusEffectCanvas = canvases.transform.FindChild("Status Effects").gameObject;
        statusEffectCanvas.SetActive(false);
        UpdateHealthBar();
    }

    public virtual void Hit(int damage, float knockbackVelocity, Vector2 knockbackDirection, float knockbackDuration)
    {
        if (this.recoveryTimeElapsed < this.hitRecoveryTime)
        { // if no damage was dealt, or if the actor is invulerable
            return; // do nothing
        }

        currentHealth -= damage; // take damage

        UpdateHealthBar();

        Knockback(knockbackVelocity, knockbackDirection, knockbackDuration); // knock the enemy backward

        this.StartFlashing(); // indicate damage by flashing
    }

    public virtual void Hit(AttackHitInfo hitInfo, Vector2 knockbackDirection) {
        if (this.recoveryTimeElapsed < this.hitRecoveryTime) { // if no damage was dealt, or if the actor is invulerable
            return; // do nothing
        }

        currentHealth -= hitInfo.damage; // take damage

        UpdateHealthBar();

        Knockback(hitInfo.knockbackVelocity, knockbackDirection, hitInfo.knockbackDuration); // knock the enemy backward
        
        this.StartFlashing(); // indicate damage by flashing
    }

    public virtual void Burn(int damage)
    {
        if (frozen)
        {
            UnFreeze(100);
        }
        if (!burning)
        {
            burning = true;
            UpdateStatusEffect(Element.Fire);
            burntickCoroutine = StartCoroutine(BurnTick(damage));
        }
    }

    public void StopBurn()
    {
        if (burning)
        {
            burning = false;
            StopCoroutine(burntickCoroutine);
            statusEffectCanvas.SetActive(false);
        }
    }

    // will probably need a StopBurn function instead,
    // so that outside sources can stop a burn

    IEnumerator BurnTick(int damage)
    {
        int count = 0;
        while (count < numBurnTicks)
        {
            currentHealth -= damage;
            UpdateHealthBar();
            count++;
            yield return new WaitForSeconds(burnTickInterval);
        }
        burning = false;
        statusEffectCanvas.SetActive(false);
    }

    public virtual void Freeze(float freezeStrength)
    {
        if (burning)
        {
            StopBurn();
        }
        if (!frozen)
        {
            if (freezePoints < 100)
            {
                freezePoints += freezeStrength;
            }
            if (freezePoints >= 100)
            {
                freezePoints = 100;
                frozen = true;
                UpdateStatusEffect(Element.Ice);
                frozenStartTime = Time.time;
            }
        }
    }

    public void UnFreeze(float thawStrength)
    {
        freezePoints -= thawStrength;
        if (frozen && freezePoints <= 0)
        {
            freezePoints = 0;
            frozen = false;
            statusEffectCanvas.SetActive(false);
            StopKnockback();
        }
    }

    public virtual void Slow()
    {
        if (!slowed)
        {
            slowed = true;
            moveSpeed *= slowFactor;
        }
    }

    public virtual void UnSlow()
    {
        if (slowed)
        {
            slowed = false;
            moveSpeed /= slowFactor;
        }
    }

    public virtual void UpdateStatusEffect(Element element)
    {
        statusEffectCanvas.transform.FindChild("Image").GetComponent<UnityEngine.UI.Image>().sprite = GameManager.S.statusEffectSprites[(int)element];
        statusEffectCanvas.SetActive(true);
    }

    public void UpdateHealthBar()
    {
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth == maxHealth)
        {
            healthBarFadeStart = Time.time;
        }
        else
        {
            healthBarCanvas.SetActive(true);
        }

        GameObject health = healthBarCanvas.transform.FindChild("Health").gameObject;
        float frac = (float)currentHealth / maxHealth;
        if (frac > 1) frac = 1;
        else if (frac < 0) frac = 0;

        // TODO lerping and derping
        Vector3 scale = new Vector3(frac, 1, 1);
        health.transform.localScale = scale;
        
        if (currentHealth <= 0)
        { // check for death
            Die();
        }
    }

    public virtual void Knockback(float knockbackValue, Vector2 knockbackDirection, float knockbackDuration) {
        knockbackDirection.Normalize(); // normalize the direction
        this.GetComponent<Rigidbody2D>().velocity = (knockbackDirection * knockbackValue); // apply the knockback force
        this.knockedBack = true; // set the knockback flag
        if (!frozen)
        {
            Invoke("StopKnockback", knockbackDuration); // stop the knockback
        }

    }

    // reset velocity to zero
    protected virtual void StopKnockback() {
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // stop motion
        this.knockedBack = false; // mark as not being knocked back
    }

    protected virtual void Die() {
        Destroy(this.gameObject);
    }

    protected void StartFlashing() {
        this.recoveryTimeElapsed = 0.0f; // reset recovery time elapsed
    }

    protected void UpdateRecovery() {
        if (this.recoveryTimeElapsed < this.hitRecoveryTime) { // if currently recovering from a hit
            this.recoveringFromHit = true; // set flag
            this.recoveryTimeElapsed += Time.deltaTime; // update elapsed time
            int flashType = (int)(this.recoveryTimeElapsed / this.hitFlashInterval); // get an int to represent flash state
            if (flashType % 2 == 0) { // if flash state is even
                this.GetComponent<SpriteRenderer>().color = this.flashColor; // flash damage color
            }
            else {
                this.GetComponent<SpriteRenderer>().color = this.originalSpriteColor; // flash the normal color
            }
        }

        if (this.recoveryTimeElapsed >= this.hitRecoveryTime) { // if the enemy is no longer recovering from a hit
            this.GetComponent<SpriteRenderer>().color = this.originalSpriteColor; // return to its normal color
            this.recoveringFromHit = false; // set flag
        }
    }

    protected virtual void UpdateMovement() {
        return;
    }

    protected virtual void Update() {
        if (!knockedBack && !frozen) {
            UpdateMovement();
        }
        UpdateRecovery();

        if (frozen)
        {
            if (Time.time - frozenStartTime > freezeDuration)
            {
                UnFreeze(100);
            }
        }
        else if (freezePoints > 0)
        {
            freezePoints -= freezeDecay * Time.deltaTime;
        }
        
        canvases.transform.rotation = Quaternion.identity;

        if (currentHealth == maxHealth && Time.time - healthBarFadeStart > healthBarFadeOutTime)
        {
            healthBarCanvas.SetActive(false);
        }
    }
}
