using UnityEngine;
using System.Collections;

public class Player : Actor {

    [HideInInspector]
    public GameObject weaponPrefab; // the initial weapon of the player
    [Header("Player Control Attributes")]
    public int playerNum = 1; // the number of the player
    public int controllerNum = 0; // the number of the controller used to control this player, 0 indicates mouse + keyboard input
    public float snapToAngle = 45f; // the minimum angle that a player can rotate at once
    public float pickupCooldown = 0.5f;
    [HideInInspector]
    public bool canRotate = true;
    [HideInInspector]
    public float playerRotationAngle = 0f; // the current rotation of the player in degrees
    [HideInInspector]
    public float currentAttackPower;
    private bool startAttacking = false; // whether or not the player is starting an attack
    // bool charging = false;
    [Header("Player Attack Attributes")]
    float chargeTime = 1f;
    float chargingFor = 0;
    private float attackCooldown; // the total duration of an attack's cooldown (set by the weapon when it attacks)
    private float attackCooldownElapsed = 0.0f; // the time elapsed since the cooldown was initiated
    [HideInInspector]
    public bool dead = false;
    public int maxMana = 10;
    [HideInInspector]
    protected int currentMana;

    //Defense vars
    public GameObject defensePrefab; // initial spell of player
    private bool defending = false;
    private bool startDefense = false;
    private float defenseCooldown;
    private float defenseCooldownElapsed = 0.0f;
    float lastManaDepletedMessage;
    float manaDepletedMessageInterval = 1;

    GameObject manaBarCanvas;
    GameObject chargeBarCanvas;
    GameObject actionIndicatorCanvas;
    GameObject playerIndicatorCanvas;
    GameObject weaponIcon;
    GameObject spellIcon;
    GameObject elementalIcon;
    GameObject goldAmountText;
    GameObject weaponGO = null;
    GameObject spellGO = null;
    GameObject grabbableItem = null;

    public Room currentRoom;

    protected override void Start()
    {
        GameObject HUD = GameManager.S.HUDCanvas.transform.FindChild("P" + playerNum + "HUD").gameObject;
        goldAmountText = GameManager.S.HUDCanvas.transform.FindChild("GoldAmount").gameObject;
        healthBarCanvas = HUD.transform.FindChild("HealthBar").gameObject;
        manaBarCanvas = HUD.transform.FindChild("ManaBar").gameObject;
        currentMana = maxMana;
        GameObject icons = HUD.transform.FindChild("Icons").gameObject;
        weaponIcon = icons.transform.FindChild("WeaponIcon").gameObject;
        spellIcon = icons.transform.FindChild("SpellIcon").gameObject;
        elementalIcon = icons.transform.FindChild("ElementalIcon").gameObject;
        HUD.SetActive(true);
        chargeBarCanvas = canvases.transform.FindChild("Charge Bar").gameObject;
        chargeBarCanvas.SetActive(false);
        actionIndicatorCanvas = canvases.transform.FindChild("Action Indicator").gameObject;
        actionIndicatorCanvas.SetActive(false);
        playerIndicatorCanvas = canvases.transform.FindChild("Player Indicator").gameObject;
        playerIndicatorCanvas.transform.FindChild("Image").GetComponent<UnityEngine.UI.Image>().sprite =
            GameManager.S.playerIndicatorSprites[playerNum - 1];


        SetWeapon(Instantiate<GameObject>(weaponPrefab));
        SetSpell(Instantiate<GameObject>(defensePrefab));

        UpdateManaBar();
        UpdateElementalIcon();
        base.Start();
    }

    public void PlacePlayer(){
        Revive(maxHealth / 2);
        currentMana = maxMana;
		Vector3 startPos = DungeonLayoutGenerator.S.levelLayout.GetComponent<DungeonLayout> ().startRoomPosition;
		gameObject.transform.position = new Vector3 (startPos.x + 10 + playerNum, startPos.y - 8, 0);
		Vector3 pos = gameObject.transform.position;

		GameObject wall = Instantiate (GameManager.S.wallFixture);
		wall.transform.position = new Vector3 (pos.x + 2, pos.y - 1, 0);

	}

    protected override void UpdateMovement() {
        if (dead) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            invincible = true;
        }

        float moveX, moveY, lookX, lookY, triggerAxis1;
        bool triggerAxis2, pickupButton;
        if (this.controllerNum > 0) { // if the player is using a controller
            // get movement input
            moveX = Input.GetAxis("P" + controllerNum + "LeftHorizontal");
            moveY = Input.GetAxis("P" + controllerNum + "LeftVertical");
            // get look input
            lookX = Input.GetAxis("P" + controllerNum + "RightHorizontal");
            lookY = Input.GetAxis("P" + controllerNum + "RightVertical");
            triggerAxis1 = Input.GetAxis("P" + controllerNum + "Fire1");
            triggerAxis2 = Input.GetButton("P" + controllerNum + "Fire2");
            pickupButton = Input.GetButtonDown("P" + controllerNum + "Pickup");
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
            triggerAxis2 = Input.GetButton("MouseFire2");
            pickupButton = Input.GetKeyDown(KeyCode.E);
        }

        HandleInput(moveX, moveY, lookX, lookY, triggerAxis1, triggerAxis2, pickupButton);

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

    void HandleInput(float moveX, float moveY, float lookX, float lookY, float triggerAxis1, bool triggerAxis2, bool pickupButton) {
        MovePlayer(moveX, moveY);

        if ((lookX != 0 || lookY != 0) && canRotate) {
            RotatePlayer(lookX, lookY);
        }

        if (attackCooldownElapsed >= attackCooldown)
        {
            if (chargeTime > 0) // if we have a charging weapon
            {
                if (triggerAxis1 < 0.0f) // charging
                {
                    weaponGO.SetActive(true);
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
        this.startDefense = triggerAxis2; // set startAttacking if the attack button is pressed
        if (this.defenseCooldownElapsed < this.defenseCooldown)
        {
            this.defenseCooldownElapsed += Time.fixedDeltaTime;
        }

        if (pickupButton)
        {
            GrabItem();
        }
    }

    public override void UpgradeElementalLevel(Element elt)
    {
        base.UpgradeElementalLevel(elt);
        weaponGO.GetComponent<Weapon>().SetElement(elt);
        UpdateElementalIcon();
    }

    public void SetWeapon(GameObject wp)
    {
        // Destroy(weaponGO);
        weaponGO = wp;
        weaponGO.transform.position = transform.position + transform.up * 0.8f;
        weaponGO.transform.rotation = transform.rotation;
        weaponGO.transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
        Weapon weapon = weaponGO.GetComponent<Weapon>();
        weapon.SetOwner(this);
        attackCooldown = weapon.cooldown;
        chargeTime = weapon.chargeTime;
        // weaponPrefab = wp;
        UpdateWeaponIcon(weapon);
        
        chargingFor = 0;
        startAttacking = false;
    }

    public void SetSpell(GameObject sp)
    {
        // Destroy(spellGO);
        spellGO = sp;
        spellGO.transform.position = transform.position;
        spellGO.transform.rotation = transform.rotation;
        spellGO.transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
        Weapon spell = spellGO.GetComponent<Weapon>();
        spell.SetOwner(this);
        defenseCooldown = spell.cooldown;
        UpdateWeaponIcon(spell);

        defending = false;
        startDefense = false;
    }

    public void UpdateWeaponIcon(Weapon weapon)
    {
        if (weapon.isSpell)
        {
            spellIcon.GetComponent<UnityEngine.UI.Image>().sprite = weapon.icon;
            spellIcon.transform.FindChild("SpellLevel").GetComponent<UnityEngine.UI.Text>().text = "LVL " + weapon.GetUpgradeLevel();
        }
        else
        {
            weaponIcon.GetComponent<UnityEngine.UI.Image>().sprite = weapon.icon;
            weaponIcon.transform.FindChild("WeaponLevel").GetComponent<UnityEngine.UI.Text>().text = "LVL " + weapon.GetUpgradeLevel();
        }
    }

    void UpdateElementalIcon()
    {
        if (element == Element.None || elementalLevel == 0)
        {
            elementalIcon.SetActive(false);
        }
        else
        {
            elementalIcon.SetActive(true);
            elementalIcon.GetComponent<UnityEngine.UI.Image>().sprite = GameManager.S.elementIcons[(int)element];
            elementalIcon.transform.FindChild("ElementalLevel").GetComponent<UnityEngine.UI.Text>().text = "LVL " + elementalLevel;
        }
    }

    void StartAttack() {
        if (attackCooldownElapsed < attackCooldown) { // if the player's attack is on cooldown or if the player is already attacking
            return;
        }

        // this.attacking = true; // mark the player as currently attacking
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        weaponGO.SetActive(true);

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
        if (spellGO == null) return;
        Weapon w = spellGO.GetComponent<Weapon>();
        int manaCost = w.manaCost;
        if (defenseCooldownElapsed < defenseCooldown || this.defending)
        { // if the player's attack is on cooldown or if the player is already attacking
            return;
        }
        if (currentMana < manaCost)
        {
            if (Time.time - lastManaDepletedMessage > manaDepletedMessageInterval)
            {
                EnqueueFloatingText("Not enough Mana!", Color.blue);
                lastManaDepletedMessage = Time.time;
            }
            return;
        }

        this.defending = true; // mark the player as currently attacking
        AddMana(manaCost * -1);
        Mathf.Clamp01(currentAttackPower);
        w.Fire(currentAttackPower);
    }

    public void AddMana(int mana)
    {
        currentMana += mana;
        if (mana > 0) EnqueueFloatingText("+" + mana + " Mana", Color.blue);
        Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaBar();
    }

    // tells the player that the most recent attack has finished
    // this method should be called by the weapon's script to indicate when it has finished attacking, and to initiate the player's cooldown
    public void StopAttack() {
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        weaponGO.GetComponent<Weapon>().ResetAttack();
        // this.attackCooldown = cooldown; // set the player's cooldown
        // Destroy(weaponGO);
        // this.attacking = false; // mark the player as not attacking
    }

    public void StopDefense(float cooldown)
    {
        this.defenseCooldownElapsed = 0.0f; // reset the cooldown
        spellGO.GetComponent<Weapon>().ResetAttack();
        // this.defenseCooldown = cooldown; // set the player's cooldown
        this.defending = false; // mark the player as not attacking
    }

    void UpdateManaBar()
    {
        if (currentMana > maxMana) currentMana = maxMana;

        GameObject mana = manaBarCanvas.transform.FindChild("Mana").gameObject;
        float frac = (float)currentMana / maxMana;
        if (frac > 1) frac = 1;
        else if (frac < 0) frac = 0;

        // TODO lerping and derping
        Vector3 scale = new Vector3(frac, 1, 1);
        mana.transform.localScale = scale;
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
        EnqueueFloatingText("+" + amount + " Gold", Color.black);
        GameManager.S.goldAmount += amount;
        goldAmountText.GetComponent<UnityEngine.UI.Text>().text = GameManager.S.goldAmount.ToString();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (dead) return;
        if (col.tag == "FloorTile")
        {
            slipping = false;
        }
        else if (col.gameObject.tag == "WeaponPickup")
        {
            WeaponPickup pickup = col.gameObject.GetComponent<WeaponPickup>();
            Weapon pickupWeapon = pickup.weaponGO.GetComponent<Weapon>();
            Weapon currentWeapon = weaponGO.GetComponent<Weapon>();
            Weapon currentSpell = spellGO.GetComponent<Weapon>();
            string actionMessage = "";
            bool upgradeFlag = (currentWeapon.weaponName == pickupWeapon.weaponName || currentSpell.weaponName == pickupWeapon.weaponName);

            if (upgradeFlag)
            {
                actionMessage += "Upgrade ";
            }
            else
            {
                actionMessage += "Switch to ";
            }
            actionMessage += pickupWeapon.weaponName;
            ShowActionMessage(actionMessage);
            grabbableItem = col.gameObject;
        }
        else if (col.tag == "Purchaseable")
        {
            Item item = col.GetComponent<Item>();
            ShowActionMessage("Buy " + item.itemName + "\n" + item.cost + " gold");
            grabbableItem = col.gameObject;
        }
    }

    void GrabItem()
    {
        if (grabbableItem == null) return;
        if (grabbableItem.gameObject.tag == "WeaponPickup")
        {
            WeaponPickup pickup = grabbableItem.GetComponent<WeaponPickup>();
            Weapon pickupWeapon = pickup.weaponGO.GetComponent<Weapon>();
            Weapon currentWeapon = weaponGO.GetComponent<Weapon>();
            Weapon currentSpell = spellGO.GetComponent<Weapon>();
            bool upgradeFlag = (currentWeapon.weaponName == pickupWeapon.weaponName || currentSpell.weaponName == pickupWeapon.weaponName);

            // swap weapons between player and pickup
            // this is prettttty bad code
            GameObject tempWeaponGO;
            if (pickupWeapon.isSpell)
            {
                if (upgradeFlag)
                {
                    tempWeaponGO = null;
                    spellGO.GetComponent<Weapon>().Upgrade();
                }
                else
                {
                    tempWeaponGO = spellGO;
                    SetSpell(pickup.weaponGO);
                    EnqueueFloatingText("Switched to " + pickupWeapon.weaponName, Color.black);
                }
            }
            else
            {
                if (upgradeFlag)
                {
                    tempWeaponGO = null;
                    weaponGO.GetComponent<Weapon>().Upgrade();
                }
                else
                {
                    tempWeaponGO = weaponGO;
                    SetWeapon(pickup.weaponGO);
                    EnqueueFloatingText("Switched to " + pickupWeapon.weaponName, Color.black);
                }
            }

            pickup.SetPickup(tempWeaponGO); // update the pickup's icon
        }
        else if (grabbableItem.gameObject.tag == "Purchaseable")
        {
            Item item = grabbableItem.GetComponent<Item>();
            if (GameManager.S.goldAmount < item.cost)
            {
                EnqueueFloatingText("Not Enough Gold!", Color.red);
            }
            else
            {
                GameManager.S.goldAmount -= item.cost;
                item.OnPlayerPickup(this); // yolo
            }
        }

        actionIndicatorCanvas.SetActive(false);
    }

    public void ShowActionMessage(string message)
    {
        actionIndicatorCanvas.transform.FindChild("Message").GetComponent<UnityEngine.UI.Text>().text = message;
        actionIndicatorCanvas.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Enemy")
        {
            Invoke("StopKnockback", 1f);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Weapon") //friendly weapon
        {
            UnFreeze(100f);
        }
        else if (col.tag == "EnemyWeapon")
        {
            UnFreeze(33f);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "WeaponPickup" || col.tag == "Purchaseable")
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
        if (dead || invincible) return;
        EnqueueFloatingText("Dead!", Color.black);
        currentHealth = 0;
        dead = true;
        healthBarCanvas.transform.FindChild("DeadText").gameObject.SetActive(true);
        healthBarCanvas.transform.FindChild("FractionText").gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().sprite = GameManager.S.tombstoneIcon;
        transform.rotation = Quaternion.identity;
        transform.FindChild("DirectionIndicator").transform.GetComponent<SpriteRenderer>().gameObject.SetActive(false);
        statusEffectCanvas.SetActive(false);
        
		GameManager.S.CheckPlayers ();
    }

    public void Revive(int hpRestore)
    {
        if (!dead) return;
        currentHealth = hpRestore;
        UpdateHealthBar();
        healthBarCanvas.transform.FindChild("DeadText").gameObject.SetActive(false);
        healthBarCanvas.transform.FindChild("FractionText").gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().sprite = GameManager.S.playerSprite;
        transform.FindChild("DirectionIndicator").transform.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
        dead = false;
    }

    public override void UpdateHealthBar()
    {
        base.UpdateHealthBar();
        healthBarCanvas.transform.FindChild("FractionText").GetComponent<UnityEngine.UI.Text>().text = currentHealth + "/" + maxHealth;
    }
}
