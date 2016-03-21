using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    public float rotationSpeed; // the speed at which the pickup rotates
    public GameObject weaponPrefab; // the weapon tied to this pickup

    private float currentRotation; // the current rotation of the pickup in degrees

	// Use this for initialization
	void Start () {
        this.currentRotation = 0.0f; // zero out the rotation
        SetPickupIcon(); // display the correct icon
	}

    void OnTriggerEnter2D(Collider2D col) {
        if (col.transform.gameObject.tag == "Player") {
            Player player = col.transform.gameObject.GetComponent<Player>();

            // swap the weapons associated with the player and the pickup
            GameObject tempPrefab = player.weaponPrefab;
            player.weaponPrefab = this.weaponPrefab;
            this.weaponPrefab = tempPrefab;
            SetPickupIcon(); // update the pickup icon
        }
    }

    void SetPickupIcon() {
        this.transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite = this.weaponPrefab.GetComponent<Weapon>().icon;
    }
	
	void FixedUpdate () {
        this.currentRotation += this.rotationSpeed; // update object's rotation angle
        this.transform.localRotation = Quaternion.AngleAxis(currentRotation, Vector3.forward); // apply the updated rotation
	}
}
