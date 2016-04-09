using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {

    public int damage = 1;  // the amount of damage this hazard does to actors
    public float knockbackVelocity = 3.0f; // the speed that this hazard knocks actors backward
    public float knockbackDuration = 0.1f; // the amount of time this hazard knocks actors backward
    public Element element;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
