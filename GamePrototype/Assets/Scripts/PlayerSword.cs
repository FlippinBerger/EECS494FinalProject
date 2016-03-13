using UnityEngine;
using System.Collections;

public class PlayerSword : MonoBehaviour {

    public float swingAngle = 90.0f; // the total angle of the swing arc
    public float swingSpeed = 5f; // the speed of the sword swing
    public int damage = 1;  // the amount of damage the sword does

    private float swordRotationAngle = 0f; // the current rotation of the sword in degrees relative to the player
    private Player parentPlayer;

    // Use this for initialization
    void Start () {
        this.swordRotationAngle = -1 * (this.swingAngle / 2f); // set the starting angle for the sword
        this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        this.transform.localPosition = new Vector3(0, 0, 0); // spawn the sword relative to the player
    }

    public void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Enemy") { // if the sword hits an enemy
            col.gameObject.GetComponent<Enemy>().TakeDamage(this.damage); // deal damage to it
        }
    }
	
	void Update () {
        // update the sword's position
        this.swordRotationAngle += swingSpeed * 100f * Time.deltaTime; // find the sword's new rotation based on swing speed
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, this.parentPlayer.playerRotationAngle + this.swordRotationAngle)); // update the sword's rotation
        Vector3 pos = new Vector3(0, 0.8f, 0); // get its distance from the center of the player
        pos = this.transform.localRotation * pos; // rotate the sword around the player
        this.transform.localPosition = pos; // set the sword's position

        if (this.swordRotationAngle >= this.swingAngle / 2f) { // if the sword has completed its arc
            this.transform.parent.gameObject.GetComponent<Player>().StopAttack(); // stop attacking
            Destroy(this.gameObject); // destroy the sword object
        }
    }
}
