using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    public bool debug = false;
    public float rotationSpeed; // the speed at which the pickup rotates
    public GameObject weaponGO; // the weapon tied to this pickup

    private float currentRotation; // the current rotation of the pickup in degrees

	// Use this for initialization
	void Start () {
        this.currentRotation = 0.0f; // zero out the rotation
        GameObject weaponPrefab;
        if (debug)
        {
            weaponPrefab = weaponGO;
        }
        else
        {
            int roll = Random.Range(0, GameManager.S.weaponDrops.Length);
            weaponPrefab = GameManager.S.weaponDrops[roll];
        }

        //instantiate a new weapon, set it to inactive
        weaponGO = (GameObject)Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        weaponGO.SetActive(false);
        SetPickup(weaponGO);
	}

    public void SetPickup(GameObject weapon) {
        if (weapon == null)
        {
            Destroy(gameObject);
            return;
        }
        weaponGO = weapon;
        weaponGO.GetComponent<Weapon>().SetOwner(null);
        this.transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite = weapon.GetComponent<Weapon>().icon;
    }
	
	void Update () {
        this.currentRotation += this.rotationSpeed; // update object's rotation angle
        this.transform.localRotation = Quaternion.AngleAxis(currentRotation, Vector3.forward); // apply the updated rotation
	}
}
