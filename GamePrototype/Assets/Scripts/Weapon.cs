using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    public int damage = 1;  // the amount of damage the weapon does
    public float cooldown = 1f; // the cooldown between attacks
    public float knockbackVelocity = 3.0f; // the velocity with which this weapon knocks enemies backward
    public float knockbackDuration = 0.1f; // the amount of time this weapon knocks enemies backward
    public Sprite icon; // the icon that represents this weapon

    protected Player parentPlayer; // the player associated with this weapon

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
