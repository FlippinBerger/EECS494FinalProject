using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public string itemDescription;
    public int cost = 0;
    public float value = 1;
    public float lifetime = 10f;
    public bool vacuum = false;
    public float vacuumThreshold = 1.5f;
    float spawntime;

    // Use this for initialization
    void Start()
    {
        spawntime = Time.time;
        itemDescription = itemDescription.Replace("\\n", "\n");
    }

    // Update is called once per frame
    void Update()
    {
        if (vacuum) Vacuum();

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

    void Vacuum()
    {
        // vacuum to player
        for (int i = 0; i < GameManager.S.players.Length; ++i)
        {
            Vector3 playerpos = GameManager.S.players[i].transform.position;
            Vector3 dist = playerpos - transform.position;
            if (dist.magnitude < vacuumThreshold)
            {
                transform.position = Vector3.Lerp(transform.position, playerpos, 0.1f);
                break; // don't care about the other player, port priority
            }
        }
    }

    abstract public void OnPlayerPickup(Player p);
}
