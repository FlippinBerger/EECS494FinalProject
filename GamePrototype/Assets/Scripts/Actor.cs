using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct FloatingTextInfo
{
    public FloatingTextInfo(string m, Color c)
    {
        message = m;
        color = c;
    }
    public string message;
    public Color color;
}

public abstract class Actor : MonoBehaviour {
    public bool debug = false;
    [Header("Actor Basic Attributes")]
    public int maxHealth; // the amount of damage the actor can take before dying
    [HideInInspector]
    public int currentHealth;
    public float moveSpeed; // the movement speed of this enemy
    [Header("Actor Hit Attributes")]
    public float hitRecoveryTime; // the time the actor spends recovering after being hit (disables enemies, makes players invincible)
    public float hitFlashInterval; // the amount of time in between color flashes when hit
    public bool invulnerableWhileRecovering = true; // whether or not this actor is invincible while recovering from a hit
    public bool flashWhileRecovering = false;
    public Color flashColor = Color.red; // the color the sprite will flash when hit
    public float healthBarFadeOutTime = 1f;
    [Header("Actor Status Effect Attributes")]
    public int baseBurnTicks = 3;
    public float burnTickInterval = 2f;
    public float freezeDuration = 5f;
    public float meltWaitTime = 1.5f;
    public float freezeDecay = 0.5f;
    public float slowFactor = 0.33f;
    [HideInInspector]
    public bool slipping = false;

    [HideInInspector]
    public Element element = Element.None;
    [HideInInspector]
    public int elementalLevel = 0;

    protected float recoveryTimeElapsed = 0.0f; // the time elapsed since hit
    protected bool knockedBack = false; // whether the enemy is currently knocked back or not
    protected bool recoveringFromHit = false; // whether the enemy is recovering or not
    protected bool burning = false;
    protected bool frozen = false;
    protected Vector3 slippingMomentum;
    protected bool slowed = false;
    Coroutine burntickCoroutine;
    protected float freezePoints = 0;
    protected float frozenStartTime;
    protected float lastFreezeTick;
    protected Color originalSpriteColor; // the default color of the sprite

    protected GameObject canvases;
    protected GameObject healthBarCanvas;
    protected GameObject statusEffectCanvas;
    float healthBarFadeStart;

    Queue<FloatingTextInfo> floatingTextQueue = new Queue<FloatingTextInfo>();
    float lastFloatingTextTime;

    protected bool invincible = false;

    void Awake()
    {
        canvases = transform.FindChild("Actor Canvases").gameObject;
    }

    // Use this for initialization
    protected virtual void Start () {
        this.originalSpriteColor = this.GetComponent<SpriteRenderer>().color; // keep track of the sprite's original color
        this.recoveryTimeElapsed = this.hitRecoveryTime; // don't start by being invulnerable
        currentHealth = maxHealth;
        statusEffectCanvas = canvases.transform.FindChild("Status Effects").gameObject;
        statusEffectCanvas.SetActive(false);
        UpdateHealthBar();
    }

    public virtual void Hit(AttackHitInfo hitInfo, Vector2 knockbackDirection) {
        if (this.recoveryTimeElapsed < this.hitRecoveryTime && invulnerableWhileRecovering) { // if no damage was dealt, or if the actor is invulerable
            return; // do nothing
        }
        currentHealth -= hitInfo.damage; // take damage
        Knockback(hitInfo.knockbackVelocity, knockbackDirection, hitInfo.knockbackDuration); // knock the enemy backward
        this.StartFlashing(); // indicate damage by flashing

        switch (hitInfo.element)
        {
            case Element.Fire:
                if (burning)
                {
                    EnqueueFloatingText("CRIT!", Color.red);
                    currentHealth -= hitInfo.damage; // take double damage
                }
                else
                {
                    Burn(1, hitInfo.elementalPower);
                }
                break;
            case Element.Ice:
                if (frozen)
                {
                    Shatter(hitInfo.elementalPower);
                }
                else
                {
                    Freeze(34 * hitInfo.elementalPower);
                }
                break;
            default:
                break;
        }

        UpdateHealthBar();
    }

    public virtual void UpgradeElementalLevel(Element elt)
    {
        if (element == elt)
        {
            elementalLevel++;
        }
        else
        {
            if (elementalLevel <= 1)
            {
                element = elt;
            }
            else
            {
                elementalLevel--;
            }
        }
    }

    public virtual void Burn(int damage, int extraTicks)
    {
        if (frozen)
        {
            UnFreeze(100);
        }
        if (!burning)
        {
            if (element == Element.Ice)
            {
                damage *= 2;
            }
            burning = true;
            EnqueueFloatingText("Burned!", Color.red);
            UpdateStatusEffect(Element.Fire, 1f);
            burntickCoroutine = StartCoroutine(BurnTick(damage, extraTicks));
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

    IEnumerator BurnTick(int damage, int extraTicks)
    {
        int count = 0;
        while (count < baseBurnTicks + extraTicks)
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
                UpdateStatusEffect(Element.Ice, freezePoints / 115f);
                lastFreezeTick = Time.time;
            }
            if (freezePoints >= 100)
            {
                if (element == Element.Fire)
                {
                    currentHealth -= maxHealth / 2;
                    UpdateHealthBar();
                }
                freezePoints = 100;
                frozen = true;
                EnqueueFloatingText("Frozen!", Color.cyan);
                UpdateStatusEffect(Element.Ice, 1f);
                frozenStartTime = Time.time;
            }
        }
    }

    public void UnFreeze(float thawStrength)
    {
        if (frozen)
        {
            freezePoints -= thawStrength;
            if (freezePoints <= 0)
            {
                freezePoints = 0;
                frozen = false;
                statusEffectCanvas.SetActive(false);
                StopKnockback();
            }
        }
    }

    protected abstract void Shatter(int elementalLevel);

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

    public void Cleanse()
    {
        StopBurn();
        UnFreeze(100);
        UnSlow();
    }

    public virtual void UpdateStatusEffect(Element element, float opacity)
    {
        UnityEngine.UI.Image image = statusEffectCanvas.transform.FindChild("Image").GetComponent<UnityEngine.UI.Image>();
        image.sprite = GameManager.S.statusEffectSprites[(int)element];
        Color c = Color.white;
        c.a = opacity;
        image.color = c;
        statusEffectCanvas.SetActive(true);
    }

    public void EnqueueFloatingText(string message, Color color)
    {
        if (floatingTextQueue.Count < 10)
        {
            floatingTextQueue.Enqueue(new FloatingTextInfo(message, color));
        }
    }

    void PopFloatingText()
    {
        if (floatingTextQueue.Count == 0 || Time.time - lastFloatingTextTime < GameManager.S.floatingTextInterval) return;
        FloatingTextInfo fti = floatingTextQueue.Dequeue();
        GameObject floatingText = (GameObject)Instantiate(GameManager.S.floatingTextPrefab, transform.position, Quaternion.identity);
        floatingText.GetComponent<TextMesh>().text = fti.message;
        floatingText.GetComponent<TextMesh>().color = fti.color;
        lastFloatingTextTime = Time.time;
    }

    public virtual void UpdateHealthBar()
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

    protected virtual void UpdateRecovery() {
        if (this.recoveryTimeElapsed < this.hitRecoveryTime) { // if currently recovering from a hit
            this.recoveringFromHit = true; // set flag
            this.recoveryTimeElapsed += Time.deltaTime; // update elapsed time
            int flashType = (int)(this.recoveryTimeElapsed / this.hitFlashInterval); // get an int to represent flash state
            if (flashType % 2 == 0 && flashWhileRecovering) { // if flash state is even
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

        PopFloatingText();

        if (frozen)
        {
            if (Time.time - frozenStartTime > freezeDuration)
            {
                UnFreeze(100);
            }
        }
        else if (freezePoints > 0 && Time.time - lastFreezeTick > meltWaitTime)
        {
            freezePoints -= freezeDecay;
            UpdateStatusEffect(Element.Ice, freezePoints / 115f);
        }
        
        canvases.transform.rotation = Quaternion.identity;

        if (currentHealth == maxHealth && 
            healthBarFadeOutTime != -1 && 
            Time.time - healthBarFadeStart > healthBarFadeOutTime)
        {
            healthBarCanvas.SetActive(false);
        }
    }
}
