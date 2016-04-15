using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

	public bool currentRoom = false;
    List<GameObject> enemies = new List<GameObject>();
	public bool set = false;

	public List<GameObject> miniMapDoors = new List<GameObject> ();

	public GameObject roomCover;

	void Start(){
		roomCover = Instantiate (GameManager.S.roomBlocker);
		roomCover.SetActive (false);
		roomCover.transform.position = gameObject.transform.position;
		GameManager.S.AddObject (roomCover);
	}

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        // if enemycount == 0  open doors
    }

    public GameObject GetClosestEnemyTo(GameObject seeker)
    {

        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(seeker.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            currentRoom = true;
            col.GetComponent<Player>().currentRoom = this;
			//SetBorder ();

			roomCover.SetActive (true);
			ShowDoors ();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            currentRoom = false;
        }
    }

	void SetBorder(){
		Vector3 pos = gameObject.transform.position;
		pos.x -= 1;
		pos.y -= 1;
		GameManager.S.currRoomBorder.transform.position = pos;
	}
		
	void ShowDoors(){
		print (miniMapDoors.Count);
		foreach (GameObject go in miniMapDoors) {
			go.SetActive (true);
		}
	}
}
