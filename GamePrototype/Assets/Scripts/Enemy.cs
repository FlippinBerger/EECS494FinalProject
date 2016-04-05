using UnityEngine;
using System.Collections;

public class Enemy : Actor {
    public float targetSelectionInterval; // the time interval on which the enemy selects a new target
    public int damage = 1;  // the amount of damage this enemy does to players
    public float knockbackVelocity = 3.0f; // the speed that this enemy knocks players backward
    public float knockbackDuration = 0.1f; // the amount of time this enemy knocks players backward

    public float aggroDistance; // the distance at which the enemy will start moving toward a player
    public float attackRange; // the max range of this enemy's attack
    public float attackRangeLeeway; // how far within the attackRange the enemy will begin trying to attack (0-1)
    public float attackCooldown; // interval between enemy attacks
    public float enrageSpeedFactor = 1.5f; // move speed multiplier for when enraged
    public float enrageDuration = 2f;

    public float directionChangeInterval = 1f; // the direction change interval while wandering

    public enum AIState { PASSIVE, AGGRO };

    public AIState aiState;

    protected float attackCooldownTimeElapsed = 0.0f; // the time elapsed since last attack
    protected GameObject target; // the target the enemy is trying to move toward
    protected float wanderHeadingAngle; // the current wander direction
    protected float targetHeadingAngle; // the target wander direction
    bool enraged = false;

	// Use this for initialization
    // TODO set to same element as room. 1/3 chance?
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
            GetComponent<SpriteRenderer>().color = GameManager.S.elementColors[(int)element];
        }
        
        healthBarCanvas = canvases.transform.FindChild("Health Bar").gameObject;
        this.aiState = AIState.PASSIVE; // start as passive

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
            Player p = col.gameObject.GetComponent<Player>();
            p.Hit(damage, knockbackVelocity, knockbackDirection, knockbackDuration, this.gameObject); // perform hit on player
            // if enemy is elemental, burn/freeze the player
            switch (element)
            {
                case Element.Fire:
                    p.Burn(1);
                    break;
                case Element.Ice:
                    p.Freeze(15);
                    break;
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        /*
        // do stuff with tiles here, like doors and lava
        if (col.gameObject.tag == "LavaTile")
        {
            Burn(1);
            Slow();
        }
        */
    }

    void OnTriggerExit2D(Collider2D col)
    {
        /*
        if (col.gameObject.tag == "LavaTile")
        {
            UnSlow();
        }
        */
    }

    public override void Hit(int damage, float knockbackVelocity, Vector2 knockbackDirection, float knockbackDuration, GameObject perpetrator) {
        this.target = perpetrator; // update target to perpetrator
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
        AggroMovement(); // move toward the target
        UpdateAttack(); // consider whether or not to attack
    }

    protected void PassiveAI() {
        PassiveMovement(); // wander

        // determine proximity aggro
        if (!recoveringFromHit) { // if the enemy can select a target
            // get the location of the closest player
            GameObject closestPlayer = EnemyAIManager.Instance.players[0];
            float distanceToClosestPlayer = float.MaxValue;
            foreach (GameObject player in EnemyAIManager.Instance.players) {
                float distance = Vector3.Distance(player.transform.position, this.transform.position);
                if (distance < distanceToClosestPlayer) {
                    closestPlayer = player;
                    distanceToClosestPlayer = distance;
                }
            }
            if (distanceToClosestPlayer <= this.aggroDistance) { // if a player is within aggro distance
                this.aiState = AIState.AGGRO; // update ai state to aggro
                this.target = closestPlayer; // target the closest player
            }
        }
    }

    protected void NewHeading() {
        this.targetHeadingAngle = Random.Range(0f, 360f); // choose the new heading
        if (this.aiState == AIState.PASSIVE) {
            Invoke("NewHeading", this.directionChangeInterval); // choose again after the interval elapses
        }
    }

    protected virtual void AggroMovement() {
        Vector3 direction = this.target.transform.position - this.transform.position; // determine the direction of the enemy's target
        direction.Normalize();

        float distanceToTarget = (this.target.transform.position - this.transform.position).magnitude;
        bool closeEnough = (distanceToTarget <= this.attackRange * this.attackRangeLeeway); // if the enemy is as close as it needs to be to attack
        if (CanAct() && !closeEnough) { // if the enemy is able and willing to move
            this.transform.position += direction * this.moveSpeed * Time.deltaTime; // move toward the target
        }
    }

    protected virtual void PassiveMovement() {
        this.wanderHeadingAngle += ((this.targetHeadingAngle - this.wanderHeadingAngle) / (this.directionChangeInterval / Time.deltaTime)); // update angle
        if (CanAct()) {
            Quaternion rotation = Quaternion.Euler(0, 0, this.wanderHeadingAngle);
            Vector3 forward = rotation * Vector3.up;
            this.transform.position += forward.normalized * this.moveSpeed * 0.3f * Time.deltaTime; // move toward the wander heading
        }
    }

    protected override void UpdateMovement() {
        ; // do nothing when called inside base.Update()
    }


    public override void Burn(int damage)
    {
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

    protected virtual void UpdateAttack() {
        this.attackCooldownTimeElapsed += Time.deltaTime; // update cooldown

        if (this.target == null || this.target.tag != "Player") { // return if target isn't valid
            return;
        }

        if (attackCooldownTimeElapsed < this.attackCooldown) { // return if attack isn't off cooldown
            return;
        }

        if (!CanAct()) { // return if can't act
            return;
        }

        float distanceToTarget = (this.target.transform.position - this.transform.position).magnitude;
        if (distanceToTarget <= this.attackRange * this.attackRangeLeeway) { // if the enemy is close enough to attack the player (with some leeway)
            Attack(this.target);
        }
        
    }

    // Update is called once per frame
    new void Update() {
        base.Update(); // call update for actor
        if (!frozen)
        {
            AI(); // handle the enemy's AI
        }
    }
}
