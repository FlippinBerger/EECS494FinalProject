using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    public bool debug = false;
    public float rotationSpeed; // the speed at which the pickup rotates
    public GameObject weaponPrefab; // the weapon tied to this pickup

    private float currentRotation; // the current rotation of the pickup in degrees

	// Use this for initialization
	void Start () {
        this.currentRotation = 0.0f; // zero out the rotation
        if (debug)
        {
            SetPickup(weaponPrefab);
        }
        else
        {
            int roll = Random.Range(0, GameManager.S.weaponDrops.Length);
            SetPickup(GameManager.S.weaponDrops[roll]); // display the correct icon
        }
	}

    public void SetPickup(GameObject weapon) {
        if (weapon == null)
        {
            Destroy(gameObject);
            return;
        }
        weaponPrefab = weapon;
        this.transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite = weapon.GetComponent<Weapon>().icon;
    }
	
	void Update () {
        this.currentRotation += this.rotationSpeed; // update object's rotation angle
        this.transform.localRotation = Quaternion.AngleAxis(currentRotation, Vector3.forward); // apply the updated rotation
	}
}
