using UnityEngine;
using System.Collections;

//This class handles each half of the hallways to transition the camera
public class Hallway : MonoBehaviour {

	public Direction dir;
	public GameObject hallCover;

	public bool set = false;

	void OnTriggerEnter2D(Collider2D col){
		if (col.CompareTag ("Player") && !set && GameManager.S.created) {
			set = true;
			hallCover.SetActive (true);
		}
	}
}
