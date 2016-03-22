using UnityEngine;
using System.Collections;

public enum Direction {
	Up, Down, Left, Right
};

public class GameManager : MonoBehaviour {

	static public GameManager S;

    // TODO make an enum and use it to index into this array
    public Sprite[] statusEffectSprites;

	//main camera
	public Camera cam;

	//room dimensions
	private int roomHeight = 12;
	private int roomWidth = 17;

	//number of players playing the game
	public int numPlayers = 0;


	void Awake(){
		S = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveCamera(Direction dir){
		Vector3 pos = cam.gameObject.transform.position;
		print (pos);

		switch (dir) {
		case Direction.Up:
			cam.gameObject.transform.position = new Vector3 (pos.x, pos.y + roomHeight, pos.z); 
			break;
		case Direction.Down:
			cam.gameObject.transform.position = new Vector3 (pos.x, pos.y - roomHeight, pos.z); 
			break;
		case Direction.Left:
			cam.gameObject.transform.position = new Vector3 (pos.x - roomWidth, pos.y, pos.z);
			break;
		case Direction.Right:
			cam.gameObject.transform.position = new Vector3 (pos.x + roomWidth, pos.y, pos.z);
			break;
		}
	}
}
