using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public int cost = 0;
    public int value = 1;
    public float lifetime = 10f;
    float spawntime;

    // Use this for initialization
    void Start()
    {
        spawntime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - spawntime;
        if (lifetime == -1) return; // -1 = infinite lifetime
        if (elapsedTime > lifetime * (4 / 5))
        {
            // TODO flicker and fade
        }
        if (elapsedTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Player p = col.GetComponent<Player>();
            if (cost == 0)
            {
                OnPlayerPickup(p);
            }
        }
    }

    abstract public void OnPlayerPickup(Player p);
}
