using UnityEngine;
using System.Collections;

public class SelectedRoomCreator : MonoBehaviour {

	public GameObject border;
	public GameObject tile;

	public int height;
	public int width;

	// Use this for initialization
	void Start () {
		border = new GameObject ("Border");
		for (int i = 0; i < height; ++i) {
			for (int j = 0; j < width; ++j) {
				if (i == 0 || i == height - 1) {
					GameObject t = Instantiate (tile);
					t.transform.position = MakePos (i, j);
					t.transform.parent = border.transform;
				} else {
					if (j == 0 || j == width - 1) {
						GameObject t = Instantiate (tile);
						t.transform.position = MakePos (i, j);
						t.transform.parent = border.transform;
					}
				}
			}
		}
	}
	
	Vector3 MakePos(int row, int col){
		return new Vector3 (col, row, 0); 
	}
}
