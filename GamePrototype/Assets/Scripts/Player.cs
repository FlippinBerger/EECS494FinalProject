﻿using UnityEngine;
using System.Collections;

public class Player : Actor {

    public GameObject weaponPrefab; // the current weapon of the player
    public int playerNum = 1; // the number of the player
    public int controllerNum = 0; // the number of the controller used to control this player, 0 indicates mouse + keyboard input
    public float playerRotationAngle = 0f; // the current rotation of the player in degrees
    private bool attacking = false; // whether or not the player is currently attacking
    private bool startAttacking = false; // whether or not the player is starting an attack
    // bool charging = false;
    float chargeTime = 1f;
    float chargingFor = 0;
    private float attackCooldown; // the total duration of an attack's cooldown (set by the weapon when it attacks)
    private float attackCooldownElapsed = 0.0f; // the time elapsed since the cooldown was initiated

    //Defense vars
    public GameObject defensePrefab;
    private bool defending = false;
    private bool startDefense = false;
    private float defenseCooldown;
    private float defenseCooldownElapsed = 0.0f;

    GameObject chargeBarCanvas;

    protected override void Start()
    {
        base.Start();

        chargeBarCanvas = canvases.transform.FindChild("Charge Bar").gameObject;
        chargeBarCanvas.SetActive(false);

        SetWeapon(weaponPrefab); // this is weird
    }

    protected override void UpdateMovement() {
        float moveX, moveY, lookX, lookY, triggerAxis1, triggerAxis2;
        if (this.controllerNum > 0) { // if the player is using a controller
            // get movement input
            moveX = Input.GetAxis("P" + controllerNum + "LeftHorizontal");
            moveY = Input.GetAxis("P" + controllerNum + "LeftVertical");
            // get look input
            lookX = Input.GetAxis("P" + controllerNum + "RightHorizontal");
            lookY = Input.GetAxis("P" + controllerNum + "RightVertical");
            triggerAxis1 = Input.GetAxis("P" + controllerNum + "Fire1");
            triggerAxis2 = Input.GetAxis("P" + controllerNum + "Fire2");
        }
        else { // if the player is using the mouse and keyboard
            // get movement input
            moveX = Input.GetAxis("MouseLeftHorizontal");
            moveY = Input.GetAxis("MouseLeftVertical");
            // get look input
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; // get the vector between the mouse and player
            lookX = -difference.x;
            lookY = difference.y;
            triggerAxis1 = -1 * Input.GetAxis("MouseFire1");
            triggerAxis2 = Input.GetAxis("MouseFire2");
        }

        HandleInput(moveX, moveY, lookX, lookY, triggerAxis1, triggerAxis2);

        if (this.startAttacking) { // if the player presses the attack button
            startAttacking = false;
            // chargingFor = 0;
            StartAttack(); // start attacking
        }

        if (this.startDefense)
        {
            StartDefense();
        }
    }

    void HandleInput(float moveX, float moveY, float lookX, float lookY, float triggerAxis1, float triggerAxis2) {
        MovePlayer(moveX, moveY);

        if (lookX != 0 || lookY != 0) {
            RotatePlayer(lookX, lookY);
        }

        if (attackCooldownElapsed >= attackCooldown)
        {
            if (chargeTime > 0) // if we have a charging weapon
            {
                if (triggerAxis1 < 0.0f) // charging
                {
                    if (chargingFor < chargeTime)
                    {
                        chargingFor += Time.deltaTime;
                    }
                }
                else if (Mathf.Approximately(triggerAxis1, 0) && chargingFor > 0) // release
                {
                    startAttacking = true;
                }
            }
            else // if we have a non-charging weapon
            {
                startAttacking = triggerAxis1 < 0.0f;
            }
        }

        UpdateChargeBar();

        if (this.attackCooldownElapsed < this.attackCooldown) { // if attacking is on cooldown
            this.attackCooldownElapsed += Time.fixedDeltaTime; // update the cooldown time elapsed
        }

        // get defense input
        this.startDefense = triggerAxis2 > 0.0f; // set startAttacking if the attack button is pressed
        if (this.defenseCooldownElapsed < this.defenseCooldown)
        {
            this.defenseCooldownElapsed += Time.fixedDeltaTime;
        }
    }

    public void SetWeapon(GameObject wp)
    {
        Weapon weapon = wp.GetComponent<Weapon>();
        attackCooldown = weapon.cooldown;
        chargeTime = weapon.chargeTime;
        weaponPrefab = wp;

        // charging = false;
        chargingFor = 0;
        startAttacking = false;
    }

    void StartAttack() {
        if (attackCooldownElapsed < attackCooldown || this.attacking) { // if the player's attack is on cooldown or if the player is already attacking
            return;
        }

        this.attacking = true; // mark the player as currently attacking
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        Instantiate(this.weaponPrefab).transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
        // TODO tell weapon how charged we are
        chargingFor = 0;
    }

    void StartDefense()
    {
        if (defenseCooldownElapsed < defenseCooldown || this.defending)
        { // if the player's attack is on cooldown or if the player is already attacking
            return;
        }

        this.defending = true; // mark the player as currently attacking
        Instantiate(this.defensePrefab).transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
    }

    // tells the player that the most recent attack has finished
    // this method should be called by the weapon's script to indicate when it has finished attacking, and to initiate the player's cooldown
    public void StopAttack() {
        // this.attackCooldownElapsed = 0.0f; // reset the cooldown
        // this.attackCooldown = cooldown; // set the player's cooldown
        this.attacking = false; // mark the player as not attacking
    }

    public void StopDefense(float cooldown)
    {
        this.defenseCooldownElapsed = 0.0f; // reset the cooldown
        this.defenseCooldown = cooldown; // set the player's cooldown
        this.defending = false; // mark the player as not attacking
    }

    public void SetChargeTime(float time)
    {
        chargeTime = time;
    }

    void UpdateChargeBar()
    {
        float amountCharged = chargingFor / chargeTime;
        if (amountCharged < 0.15f || chargeTime == 0)
        {
            chargeBarCanvas.SetActive(false);
            chargeBarCanvas.transform.FindChild("Border").GetComponent<UnityEngine.UI.Image>().color = Color.black;
            return;
        }

        chargeBarCanvas.SetActive(true);

        if (amountCharged >= 1)
        {
            amountCharged = 1;
            chargeBarCanvas.transform.FindChild("Border").GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        }

        chargeBarCanvas.transform.FindChild("Charge").localScale = new Vector3(amountCharged, 1, 1);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 knockbackDirection = this.transform.position - col.gameObject.transform.position; // determine direction of knockback
        if (col.gameObject.tag == "Enemy") { // if hit by an enemy
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            Hit(enemy.damage, enemy.knockbackVelocity, knockbackDirection, enemy.knockbackDuration); // perform hit on player
        }
        else if (col.gameObject.tag == "Hazard")
        {
            Hazard hazard = col.gameObject.GetComponent<Hazard>();
            Knockback(hazard.knockbackVelocity, knockbackDirection, hazard.knockbackDuration);
            Burn(1);
            Destroy(col.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // do stuff with tiles here, like doors and lava
        if (col.gameObject.tag == "LavaTile")
        {
            Burn(1);
            Slow();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "LavaTile")
        {
            UnSlow();
        }
    }

    void MovePlayer(float horizontal, float vertical) {
        Vector3 movement = new Vector3(horizontal, vertical);
        movement *= moveSpeed;
        transform.position += movement * Time.deltaTime;
    }

    void RotatePlayer(float horizontal, float vertical) {
        if ((new Vector2(horizontal, vertical)).magnitude < 0.5f) return;
        this.playerRotationAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.playerRotationAngle));
    }
}
