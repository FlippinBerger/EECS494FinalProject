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

    public void SetPickupIcon() {
        this.transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite = this.weaponPrefab.GetComponent<Weapon>().icon;
    }
	
	void FixedUpdate () {
        this.currentRotation += this.rotationSpeed; // update object's rotation angle
        this.transform.localRotation = Quaternion.AngleAxis(currentRotation, Vector3.forward); // apply the updated rotation
	}
}
