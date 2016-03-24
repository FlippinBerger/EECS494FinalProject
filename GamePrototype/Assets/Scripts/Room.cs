using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

	public Direction[] doors;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(Direction[] d){
		doors = d;
	}

	public bool HasDoors(Direction[] dirs){
		bool flag = false;
		foreach (Direction d in doors)
			print (d);
		print ("Done");
		foreach (Direction d in dirs) {
			print (d);
			for (int i = 0; i < doors.Length; ++i) {
				if (doors [i] == d || d == Direction.None) {
					flag = true;
					break;
				}
			}
			if (!flag) {
				return false;
			}
			flag = false;
		}
		return true;
	}

}
