using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Actor {
    [Header("Enemy Basic Attributes")]
    public int damage = 1;  // the amount of damage this enemy does to players
    public float knockbackVelocity = 3.0f; // the speed that this enemy knocks players backward
    public float knockbackDuration = 0.1f; // the amount of time this enemy knocks players backward
    public float attackCooldown; // interval between enemy attacks

    [Header("Enemy AI Attributes")]
    public float aggroDistance; // the distance at which the enemy will start moving toward a player
    public float aggroDuration; // how long aggro state lasts after a player is outside of aggro range before switching to passive AI
    public float attackRange; // the max range of this enemy's attack
    public float maxAttackWaitTime; // how long this enemy can wait before it is forced to attack (when off cooldown)
    public float attackRangeLeeway; // how far within the attackRange the enemy will begin trying to attack (0-1)
    public float directionChangeInterval = 1f; // the direction change interval while wandering
    public float wanderRadius = 2f; // the radius this enemy will wander
    public float movementAngleRange; // the variance in the enemy's movement

    [Header("Enemy Enrage Attributes")]
    public float enrageSpeedFactor = 1.5f; // move speed multiplier for when enraged
    public float enrageDuration = 2f;

    [Header("Enemy Attributes Scaling")]
    public int attackScalingAmount; // the factor that this attribute is scaled by each level
    public int healthScalingAmount;


    public enum AIState { PASSIVE, AGGRO };

    [HideInInspector]
    public AIState aiState;
    [HideInInspector]
    public GameObject homeTile; // the spawning tile of the enemy

    protected float attackCooldownTimeElapsed = 0.0f; // the time elapsed since last attack
    protected GameObject target; // the target the enemy is trying to move toward
    protected float wanderHeadingAngle; // the current wander direction
    protected float targetHeadingAngle; // the target wander direction
    protected float aggroRandomAngle; // the random angle used in aggro
    protected float aggroRandomDirection; // the random direction used in aggro
    protected float aggroTimer; // how long since the aggro has been refreshed
    bool enraged = false;

	// Use this for initialization
	protected override void Start () {
        // start in passive AI
        this.PassiveAI();
        NewHeading();

        // set element
        int roll = Random.Range(0, 2);
        if (roll == 0)
        {
            // become elemental
            element = GameManager.S.currentLevelElement;
            // elementalLevel = GameManager.S.round;
            GetComponent<SpriteRenderer>().color = GameManager.S.elementColors[(int)element];
        }
        
        healthBarCanvas = canvases.transform.FindChild("Health Bar").gameObject;
        this.aiState = AIState.PASSIVE; // start as passive

        maxHealth += (healthScalingAmount * (GameManager.S.round - 1)); // health upgrades every 2 levels

        RandomizeAttackTime();

        base.Start(); // call start for actor
    }

    protected bool CanAct() {
        if (this.frozen) {
            return false;
        }

        if (this.knockedBack) {
            return false;
        }

        if (this.recoveringFromHit) {
            return false;
        }

        return true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 knockbackDirection = this.transform.position - col.gameObject.transform.position; // determine direction of knockback
        if (col.gameObject.tag == "Player")
        {
            Invoke("StopKnockback", 1f);
            Player p = col.gameObject.GetComponent<Player>();
            if (!frozen)
            {
                p.Hit(new AttackHitInfo(damage, knockbackVelocity, knockbackDuration, element, elementalLevel, this.gameObject),
                      knockbackDirection); // perform hit on player
            }
        }
        else if (col.gameObject.tag == "Enemy" && !knockedBack) // will only collide with enemies if the other enemy was knockedback
        {
            Invoke("StopKnockback", 1f);
            float velocity = col.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            Hit(new AttackHitInfo((int)velocity / 3, velocity, 0.5f, null), knockbackDirection);
        }
    }

    public override void Hit(AttackHitInfo hitInfo, Vector2 knockbackDirection)
    {
        base.Hit(hitInfo, knockbackDirection);
        if (hitInfo.source != null)
        {
            target = hitInfo.source;
        }
    }

    protected override void Shatter(int elementalLevel)
    {
        int numShardsPerLevel = 5;
        int damagePerLevel = 2;
        EnqueueFloatingText("SHATTERED!", Color.cyan);
        for (int i = 0; i < elementalLevel * numShardsPerLevel; ++i)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            GameObject shardGO = (GameObject)Instantiate(GameManager.S.iceShardPrefab, transform.position, rotation);
            Projectile p = shardGO.GetComponent<Projectile>();
            p.SetHitInfo(new AttackHitInfo(damagePerLevel * elementalLevel, 5, 0.5f, Element.Ice, elementalLevel, null));
            p.SetMissileInfo(1, 8, 8, 5, 5);
        }
        Die();
    }

    public override void Knockback(float knockbackValue, Vector2 knockbackDirection, float knockbackDuration)
    {
        base.Knockback(knockbackValue, knockbackDirection, knockbackDuration);
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    protected override void StopKnockback()
    {
        base.StopKnockback();
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    protected void AI() {
        if (this.aiState == AIState.PASSIVE) {
            PassiveAI();
        }
        else if (this.aiState == AIState.AGGRO) {
            AggroAI();
        }
    }

    protected void AggroAI() {
        UpdateAttack(); // consider whether or not to attack
        float distanceToClosestPlayer = Vector3.Distance(GetClosestPlayer().transform.position, this.transform.position);
        if (distanceToClosestPlayer > this.aggroDistance) // if players are too far away
        {
            this.aggroTimer += Time.deltaTime; // update aggro timer
            if (this.aggroTimer > this.aggroDuration) // if the aggro timer runs out
            {
                this.aiState = AIState.PASSIVE; // revert to passive AI
            }
        }
        else if (distanceToClosestPlayer <= this.aggroDistance)
        {
            this.aggroTimer = 0f; // reset the aggro timer
        }
    }

    protected GameObject GetClosestPlayer()
    {
        GameObject closestPlayer = GameManager.S.players[0];
        float distanceToClosestPlayer = float.MaxValue;
        foreach (GameObject player in EnemyAIManager.Instance.players)
        {
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            if (distance < distanceToClosestPlayer && !player.GetComponent<Player>().dead)
            {
                closestPlayer = player;
                distanceToClosestPlayer = distance;
            }
        }

        return closestPlayer;
    }

    protected void PassiveAI() {
        GameObject closestPlayer = GetClosestPlayer();
        float distanceToClosestPlayer = Vector3.Distance(closestPlayer.transform.position, this.transform.position); // get distance to closest player

        // determine proximity aggro
        if (!recoveringFromHit) { // if the enemy can select a target
            // get the location of the closest player
           
            if (distanceToClosestPlayer <= this.aggroDistance) { // if a player is within aggro distance
                this.aiState = AIState.AGGRO; // update ai state to aggro
                this.target = closestPlayer; // target the closest player
            }
        }
    }

    protected void NewHeading() {
        this.targetHeadingAngle = Random.Range(0f, 360f); // choose the new heading for wandering
        this.aggroRandomAngle = Random.Range(-this.movementAngleRange, this.movementAngleRange); // gets a random angle for moving while aggro'ed
        // randomly select which direction perpendicular to the player to move
        if (Random.Range(0f, 1f) > 0.5f) {
            this.aggroRandomDirection = -1;
        }
        else {
            this.aggroRandomDirection = 1;
        }
        Invoke("NewHeading", this.directionChangeInterval); // choose again after the interval elapses
    }

    protected virtual void AggroMovement() {
        if (target == null) return;
        Vector3 direction = this.target.transform.position - this.transform.position; // determine the direction of the enemy's target
        direction.Normalize();

        Quaternion randomRotation = Quaternion.AngleAxis(this.aggroRandomAngle, Vector3.forward); // turn the random aggro angle into a Quaternion

        float distanceToTarget = (this.target.transform.position - this.transform.position).magnitude;
        bool closeEnough = (distanceToTarget <= this.attackRange * this.attackRangeLeeway); // if the enemy is as close as it needs to be to attack
        if (CanAct()) { // if the enemy is able to move
            if (!closeEnough) { // if the enemy isn't close enough to attack
                this.transform.position += randomRotation * direction * this.moveSpeed * Time.deltaTime; // move toward the target with variance
            }
            else if (closeEnough) { // if the enemy is close enough to attack
                Quaternion perpendicular = Quaternion.AngleAxis(90f * this.aggroRandomDirection, Vector3.forward); // choose a random perpendicular direction
                this.transform.position += randomRotation * perpendicular * direction * this.moveSpeed * Time.deltaTime; // move perpendicular to the target with variance
            }
        }
    }

    protected virtual void PassiveMovement() {
        float distanceToHomeTile = Vector3.Distance(this.transform.position, this.homeTile.transform.position); // find the distance to home tile
        if (distanceToHomeTile > this.wanderRadius) // if the enemy is too far from home
        {
            CancelInvoke("NewHeading"); // cancel the random wandering
            Vector3 directionVector = this.homeTile.transform.position - this.transform.position;
            this.targetHeadingAngle = Mathf.Atan2(directionVector.x, directionVector.y); // head toward home for the direction interval
            Invoke("NewHeading", this.directionChangeInterval); // restart random heading selection
        }
        this.wanderHeadingAngle += ((this.targetHeadingAngle - this.wanderHeadingAngle) / (this.directionChangeInterval / Time.deltaTime)); // update angle

        if (CanAct()) {
            Quaternion rotation = Quaternion.Euler(0, 0, this.wanderHeadingAngle);
            Vector3 forward = rotation * Vector3.up;
            this.transform.position += forward.normalized * this.moveSpeed * 0.3f * Time.deltaTime; // move toward the wander heading
        }
    }

    protected override void UpdateMovement() {
        if (this.aiState == AIState.PASSIVE) {
            PassiveMovement(); // wander
        }
        if (this.aiState == AIState.AGGRO) {
            AggroMovement(); // move toward the target
        }
        
    }


    public override void Burn(int damage)
    {
        if (element == Element.Fire)
        {
            damage = -1; // resist burn
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

    protected void Enrage()
    {
        if (!enraged)
        {
            enraged = true;
            moveSpeed *= enrageSpeedFactor;
            Invoke("UnEnrage", enrageDuration);
        }
    }

    protected void UnEnrage()
    {
        if (enraged)
        {
            enraged = false;
            moveSpeed /= enrageSpeedFactor;
        }
    }

    protected virtual void Attack(GameObject target) {
        this.attackCooldownTimeElapsed = 0.0f; // reset cooldown
    }

    void RandomizeAttackTime()
    {
        attackCooldownTimeElapsed -= Random.Range(0, maxAttackWaitTime);
    }

    protected virtual void UpdateAttack() {
        this.attackCooldownTimeElapsed += Time.deltaTime; // update cooldown

        if (this.target == null || this.target.tag != "Player") { // return if target isn't valid
            return;
        }

        Player p = target.GetComponent<Player>();
        if (p.dead)
        {
            target = GetClosestPlayer();
            return;
        }

        if (attackCooldownTimeElapsed < this.attackCooldown) { // return if attack isn't off cooldown
            return;
        }

        if (!CanAct()) { // if the enemy can't act
            this.attackCooldownTimeElapsed = 0.0f; // trigger the attack cooldown
            return; // don't attack
        }

        float distanceToTarget = (this.target.transform.position - this.transform.position).magnitude;
        if (distanceToTarget <= this.attackRange * this.attackRangeLeeway) { // if the enemy is close enough to attack the player (with some leeway)
            Attack(this.target);
        }
        
    }

    // Update is called once per frame
    new void Update() {
        base.Update(); // call update for actor
        if (!frozen && transform.parent != null && transform.parent.GetComponent<Room>().currentRoom)
        {
            AI(); // handle the enemy's AI
        }
    }

    protected override void Die()
    {
        Vector3 pos = transform.position;
        Destroy(this.gameObject);
        List<GameObject> drops = new List<GameObject>();
        drops.Add((GameObject)Instantiate(GameManager.S.coinPrefab, pos, Quaternion.identity));

        // roll for weapon drop
        int roll = Random.Range(0, 6);
        if (debug) roll = 0;
        if (roll == 0)
        {
            GameObject wp = (GameObject)Instantiate(GameManager.S.weaponPickupPrefab, pos, Quaternion.identity);
            wp.transform.parent = transform.parent;
            drops.Add(wp);
        }

        // roll for mana drop
        roll = Random.Range(0, 3);
        if (debug) roll = 0;
        if (roll == 0)
        {
            GameObject manapot = (GameObject)Instantiate(GameManager.S.manaPotionPrefab, pos, Quaternion.identity);
            manapot.transform.parent = transform.parent;
            drops.Add(manapot);
        }

        int numDrops = drops.Count;
        float angleDiff = 360f / numDrops;
        float initialAngle = Random.Range(0, 360);
        Vector3 offset = Vector3.right * 0.35f;

        // if more than one drop, spread them out
        if (numDrops > 1)
        {
            for (int i = 0; i < numDrops; ++i)
            {
                drops[i].transform.position += Quaternion.Euler(0, 0, (initialAngle + (angleDiff * i))) * offset;
            }
        }
        transform.parent.GetComponent<Room>().RemoveEnemy(this.gameObject);
    }
}
