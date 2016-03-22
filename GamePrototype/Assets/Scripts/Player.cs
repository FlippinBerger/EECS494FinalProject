using UnityEngine;
using System.Collections;

public class Player : Actor {

    public GameObject weaponPrefab; // the current weapon of the player
    public int playerNum = 1; // the number of the player
    public int controllerNum = 0; // the number of the controller used to control this player, 0 indicates mouse + keyboard input
    public float playerRotationAngle = 0f; // the current rotation of the player in degrees
    private bool attacking = false; // whether or not the player is currently attacking
    private bool startAttacking = false; // whether or not the player is starting an attack
    private float attackCooldown; // the total duration of an attack's cooldown (set by the weapon when it attacks)
    private float attackCooldownElapsed = 0.0f; // the time elapsed since the cooldown was initiated

    protected override void UpdateMovement() {
        if (this.controllerNum > 0) { // if the player is using a controller
            HandleControllerInput();
        }
        else { // if the player is using the mouse and keyboard
            HandleMouseInput();
        }

        if (this.startAttacking) { // if the player presses the attack button
            StartAttack(); // start attacking
        }
    }

    void HandleMouseInput() {
        // get movement input
        float moveX = Input.GetAxis("MouseLeftHorizontal");
        float moveY = Input.GetAxis("MouseLeftVertical");
        MovePlayer(moveX, moveY);

        // get look input
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; // get the vector between the mouse and player
        difference.Normalize(); // normalize the vector
        this.playerRotationAngle = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - 90f; // get the z-rotation corresponding to the difference vector
        transform.rotation = Quaternion.Euler(0f, 0f, this.playerRotationAngle); // rotate the player object to look at the mouse

        // get attack input
        this.startAttacking = Input.GetAxis("MouseFire1") > 0.0f; // set startAttacking if the attack button is pressed
        if (this.attackCooldownElapsed < this.attackCooldown) { // if attacking is on cooldown
            this.attackCooldownElapsed += Time.fixedDeltaTime; // update the cooldown time elapsed
        }
    }

    void HandleControllerInput() {
        // get movement input
        float moveX = Input.GetAxis("P" + controllerNum + "LeftHorizontal");
        float moveY = Input.GetAxis("P" + controllerNum + "LeftVertical");
        MovePlayer(moveX, moveY);

        // get look input
        float lookX = Input.GetAxis("P" + controllerNum + "RightHorizontal");
        float lookY = Input.GetAxis("P" + controllerNum + "RightVertical");
        if (lookX != 0 || lookY != 0) {
            RotatePlayer(lookX, lookY);
        }

        // get attack input
        this.startAttacking = Input.GetAxis("P" + controllerNum + "Fire1") < 0.0f; // set startAttacking if the attack button is pressed
        if (this.attackCooldownElapsed < this.attackCooldown) { // if attacking is on cooldown
            this.attackCooldownElapsed += Time.fixedDeltaTime; // update the cooldown time elapsed
        }
    }

    void StartAttack() {
        if (attackCooldownElapsed < attackCooldown || this.attacking) { // if the player's attack is on cooldown or if the player is already attacking
            return;
        }

        this.attacking = true; // mark the player as currently attacking
        Instantiate(this.weaponPrefab).transform.parent = this.gameObject.transform; // instantiate the weapon with this player as its parent
    }

    // tells the player that the most recent attack has finished
    // this method should be called by the weapon's script to indicate when it has finished attacking, and to initiate the player's cooldown
    public void StopAttack(float cooldown) {
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        this.attackCooldown = cooldown; // set the player's cooldown
        this.attacking = false; // mark the player as not attacking
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 knockbackDirection = this.transform.position - col.gameObject.transform.position; // determine direction of knockback
        if (col.gameObject.tag == "Enemy") { // if hit by an enemy
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            Hit(enemy.damage, enemy.knockbackVelocity, knockbackDirection, enemy.knockbackDuration); // perform hit on player
        }
        else if (col.gameObject.tag == "Fireball")
        {
            // TODO make these serializable values
            Knockback(5f, knockbackDirection, 0.2f);
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
        this.playerRotationAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.playerRotationAngle));
    }
}
