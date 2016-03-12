using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float moveSpeed = 5.0f;
    public float swingAngle = 90.0f;
    public float swingSpeed = 5f;
    public float attackCooldown;
    public GameObject swordPrefab;

    private float playerRotationAngle = 0f; // the current rotation of the player in degrees
    private float swordRotationAngle = 0f; // the current rotation of the sword in degrees relative to the player
    private bool attacking = false; // whether or not the player is currently attacking
    private float attackCooldownElapsed = 0.0f;
    private GameObject sword;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() {
        float moveX = Input.GetAxis("P1LeftHorizontal");
        float moveY = Input.GetAxis("P1LeftVertical");
        MovePlayer(moveX, moveY);

        float lookX = Input.GetAxis("P1RightHorizontal");
        float lookY = Input.GetAxis("P1RightVertical");
        if (lookX != 0 || lookY != 0) {
            RotatePlayer(lookX, lookY);
        }

        float attack = Input.GetAxis("Fire1");
        if (this.attackCooldownElapsed < this.attackCooldown) { // if attacking is on cooldown
            this.attackCooldownElapsed += Time.fixedDeltaTime; // update the cooldown time elapsed
        }
        if ((attack != 0 || this.attacking) && this.attackCooldownElapsed >= this.attackCooldown) {
            Attack();
        }


    }

    void Attack() {
        
        if (attacking == false) {
            print("Making sword!");
            this.sword = Instantiate(swordPrefab); // instantiate the sword prefab
            this.swordRotationAngle = -1 * (this.swingAngle / 2f);
            attacking = true;
        }

        // update the sword's position
        this.swordRotationAngle += swingSpeed; ; // find the sword's new rotation based on swing speed
        sword.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.playerRotationAngle + this.swordRotationAngle)); // update the sword's rotation
        Vector3 pos = new Vector3(0, 0.8f, 0); // get its distance from the center of the player
        pos = sword.transform.rotation * pos; // rotate the sword around the player
        pos += transform.position; // center the sword on the player
        sword.transform.position = pos; // set the sword's position

        if (this.swordRotationAngle >= this.swingAngle/2f) { // if the sword has completed its arc
            this.attacking = false; // stop attacking
            Destroy(this.sword); // destroy the sword object
            this.attackCooldownElapsed = 0.0f; // trigger the cooldown
        }
    }

    void MovePlayer(float horizontal, float vertical) {
        Vector3 movement = new Vector3(horizontal, vertical);
        movement *= moveSpeed;
        transform.position += movement * Time.fixedDeltaTime;
    }

    void RotatePlayer(float horizontal, float vertical) {
        this.playerRotationAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.playerRotationAngle));
    }
}
