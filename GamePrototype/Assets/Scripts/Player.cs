using UnityEngine;
using System.Collections;

public class Player : Actor {

    public GameObject weaponPrefab; // the current weapon of the player
    public int playerNum = 1; // the number of the player
    public int controllerNum = 0; // the number of the controller used to control this player, 0 indicates mouse + keyboard input
    public float playerRotationAngle = 0f; // the current rotation of the player in degrees
    public float snapToAngle = 45f; // the minimum angle that a player can rotate at once
    public float currentAttackPower;
    // private bool attacking = false; // whether or not the player is currently attacking
    private bool startAttacking = false; // whether or not the player is starting an attack
    // bool charging = false;
    float chargeTime = 1f;
    float chargingFor = 0;
    private float attackCooldown; // the total duration of an attack's cooldown (set by the weapon when it attacks)
    private float attackCooldownElapsed = 0.0f; // the time elapsed since the cooldown was initiated
    public bool dead = false;

    //Defense vars
    public GameObject defensePrefab;
    private bool defending = false;
    private bool startDefense = false;
    private float defenseCooldown;
    private float defenseCooldownElapsed = 0.0f;

    GameObject chargeBarCanvas;
    GameObject actionIndicatorCanvas;
    GameObject goldAmountText;
    GameObject weaponGO = null;
    GameObject tombstoneGO = null;
    int goldAmount = 0;

    protected override void Start()
    {
        GameObject HUD = GameManager.S.HUDCanvas.transform.FindChild("P" + playerNum + "HUD").gameObject;
        healthBarCanvas = HUD.transform.FindChild("Health Bar").gameObject;
        goldAmountText = HUD.transform.FindChild("GoldAmount").gameObject;
        HUD.SetActive(true);
        chargeBarCanvas = canvases.transform.FindChild("Charge Bar").gameObject;
        chargeBarCanvas.SetActive(false);
        actionIndicatorCanvas = canvases.transform.FindChild("Action Indicator").gameObject;
        actionIndicatorCanvas.SetActive(false);

        SetWeapon(weaponPrefab); // this is weird

        base.Start();
    }

    public void PlacePlayer(int playerNum){
		Vector3 startPos = DungeonLayoutGenerator.S.levelLayout.GetComponent<DungeonLayout> ().startRoomPosition;
		gameObject.transform.position = new Vector3 (startPos.x + 10 + playerNum, startPos.y - 8, 0);
	}

    protected override void UpdateMovement() {
        if (dead) return;

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
                    if (weaponGO == null)
                    {
                        //spawn that baby
                        weaponGO = (GameObject)Instantiate(this.weaponPrefab, transform.position + transform.up * .8f, transform.rotation);
                        weaponGO.transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
                    }
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
                if (triggerAxis1 < 0f)
                {
                    startAttacking = true;
                }
                else if (Mathf.Approximately(triggerAxis1, 0) && weaponGO != null)
                {
                    StopAttack();
                }
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
        Destroy(weaponGO);
        Weapon weapon = wp.GetComponent<Weapon>();
        attackCooldown = weapon.cooldown;
        chargeTime = weapon.chargeTime;
        weaponPrefab = wp;

        // charging = false;
        chargingFor = 0;
        startAttacking = false;
    }

    void StartAttack() {
        if (attackCooldownElapsed < attackCooldown) { // if the player's attack is on cooldown or if the player is already attacking
            return;
        }

        // this.attacking = true; // mark the player as currently attacking
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        
        if (weaponGO == null)
        {
            weaponGO = (GameObject)Instantiate(this.weaponPrefab, transform.position, transform.rotation);
            weaponGO.transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
        }

        if (chargeTime == 0)
        {
            this.currentAttackPower = 0;
        }
        else
        {
            this.currentAttackPower = chargingFor / chargeTime;
        }
        Mathf.Clamp01(currentAttackPower);
        weaponGO.GetComponent<Weapon>().Fire(currentAttackPower);

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
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        // this.attackCooldown = cooldown; // set the player's cooldown
        Destroy(weaponGO);
        // this.attacking = false; // mark the player as not attacking
    }

    public void StopDefense(float cooldown)
    {
        this.defenseCooldownElapsed = 0.0f; // reset the cooldown
        this.defenseCooldown = cooldown; // set the player's cooldown
        this.defending = false; // mark the player as not attacking
    }

    void UpdateChargeBar()
    {
        this.currentAttackPower = chargingFor / chargeTime;
        if (this.currentAttackPower < 0.15f || chargeTime == 0)
        {
            chargeBarCanvas.SetActive(false);
            chargeBarCanvas.transform.FindChild("Border").GetComponent<UnityEngine.UI.Image>().color = Color.black;
            return;
        }

        chargeBarCanvas.SetActive(true);

        if (this.currentAttackPower >= 1)
        {
            this.currentAttackPower = 1;
            chargeBarCanvas.transform.FindChild("Border").GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        }

        chargeBarCanvas.transform.FindChild("Charge").localScale = new Vector3(this.currentAttackPower, 1, 1);
    }

    public void AddGold(int amount)
    {
        // TODO floating gold text
        goldAmount += amount;
        goldAmountText.GetComponent<UnityEngine.UI.Text>().text = goldAmount.ToString();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "FloorTile")
        {
            slipping = false;
        }
        else if (col.gameObject.tag == "EnemyWeapon") {
            EnemyWeapon enemyWeapon = col.gameObject.GetComponent<EnemyWeapon>();
            Vector2 knockbackDirection = this.transform.position - enemyWeapon.parentEnemy.transform.position; // determine direction of knockback
            Hit(enemyWeapon.damage, enemyWeapon.knockbackVelocity, knockbackDirection, enemyWeapon.knockbackDuration, enemyWeapon.parentEnemy.gameObject); // perform hit on player
        }
        else if (col.gameObject.tag == "WeaponPickup")
        {
            WeaponPickup pickup = col.gameObject.GetComponent<WeaponPickup>();
            actionIndicatorCanvas.transform.FindChild("Message").GetComponent<UnityEngine.UI.Text>().text = pickup.weaponPrefab.GetComponent<Weapon>().weaponName;
            actionIndicatorCanvas.SetActive(true);

            if (Input.GetButtonDown("P" + controllerNum + "Pickup")) {
                // swap weapons between player and pickup
                GameObject tempPrefab = this.weaponPrefab;
                SetWeapon(pickup.weaponPrefab);
                pickup.weaponPrefab = tempPrefab;

                pickup.SetPickup(tempPrefab); // update the pickup's icon
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Weapon") //friendly weapon
        {
            UnFreeze(100f);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "WeaponPickup")
        {
            actionIndicatorCanvas.SetActive(false);
        }
    }

    void MovePlayer(float horizontal, float vertical) {
        Vector3 movement = new Vector3(horizontal, vertical);
        movement *= moveSpeed;
        if (!slipping)
        {
            slippingMomentum = movement;
        }
        else
        {
            slippingMomentum = Vector3.MoveTowards(slippingMomentum, movement, 0.05f);
        }
        transform.position += slippingMomentum * Time.deltaTime;
    }

    void RotatePlayer(float horizontal, float vertical) {
        if ((new Vector2(horizontal, vertical)).magnitude < 0.5f) return;
        this.playerRotationAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
        float remainder = playerRotationAngle % snapToAngle;
        if (remainder > snapToAngle / 2f)
        {
            playerRotationAngle += (snapToAngle - remainder);
        }
        else
        {
            playerRotationAngle -= remainder;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.playerRotationAngle));
    }

    protected override void Die()
    {
        dead = true;
        healthBarCanvas.transform.FindChild("DeadText").gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().sprite = GameManager.S.tombstoneIcon;
        transform.FindChild("DirectionIndicator").transform.GetComponent<SpriteRenderer>().gameObject.SetActive(false);
        statusEffectCanvas.SetActive(false);
        
		GameManager.S.CheckPlayers ();
    }
}
