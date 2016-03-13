using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float moveSpeed = 5.0f;

    public float attackCooldown;
    public GameObject weaponPrefab;
    public int playerNum = 1;
    public float playerRotationAngle = 0f; // the current rotation of the player in degrees

    private bool attacking = false; // whether or not the player is currently attacking
    private float attackCooldownElapsed = 0.0f; // the time elapsed since the cooldown was initiated

	// Use this for initialization
	void Start () {
	
	}

    void Update() {
        float moveX = Input.GetAxis("P" + playerNum + "LeftHorizontal");
        float moveY = Input.GetAxis("P" + playerNum + "LeftVertical");
        MovePlayer(moveX, moveY);

        float lookX = Input.GetAxis("P" + playerNum + "RightHorizontal");
        float lookY = Input.GetAxis("P" + playerNum + "RightVertical");
        if (lookX != 0 || lookY != 0) {
            RotatePlayer(lookX, lookY);
        }

        float attack = Input.GetAxis("P" + playerNum + "Fire1");
        if (this.attackCooldownElapsed < this.attackCooldown) { // if attacking is on cooldown
            this.attackCooldownElapsed += Time.fixedDeltaTime; // update the cooldown time elapsed
        }
        if (attack != 0) { // if the player presses the attack button
            StartAttack(); // start attacking
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
    public void StopAttack() {
        this.attackCooldownElapsed = 0.0f; // reset the cooldown
        this.attacking = false;
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
