using UnityEngine;
using System.Collections;

abstract public class HazardTile : MonoBehaviour
{
    public GameObject hazardPrefab;
    public GameObject dangerIndicatorPrefab;

    public int health = 4;
    public float hazardMinRadius = 2f;
    public float hazardMaxRadius = 3f;
    public float eruptionBuildupTime = 1f;
    public float eruptionDormantTime = 2f;
    public float maxWaitTime = 1.5f;
    public int numHazardsPerEruption = 3;
    public Element element;

    protected float lastPhaseChange;
    protected bool eruptionPrepared = false;
    protected float delay = 0;

    public float shakeDuration = .75f;
    float shakeStartTime;
    float shakeSpeed = 2;
    SpriteRenderer sprenderer;

    void Awake()
    {
        sprenderer = transform.FindChild("Sprite").GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        delay = Random.Range(0, maxWaitTime);
        lastPhaseChange = -1;
    }

    // Update is called once per frame
    void Update()
    {
        // shake
        float time = Time.time;
        if (time - shakeStartTime < shakeDuration)
        {
            Vector3 pos = sprenderer.transform.localPosition;
            pos.x += shakeSpeed * Time.deltaTime;
            sprenderer.transform.localPosition = pos;

            if (pos.x >= 0.05f || pos.x <= -0.05f)
            {
                shakeSpeed *= -1;
            }
        }
        else
        {
            sprenderer.transform.position = transform.position;
        }

        // everything else
        Room room = transform.parent.GetComponent<Room>();
        if (transform.parent == null) return;
        if (room.currentRoom)
        {
            if (lastPhaseChange < 0)
            {
                lastPhaseChange = Time.time;
            }
        }
        else
        {
            // this is somewhat abusable
            // lastPhaseChange = -1;
            return;
        }
        if (room.IsCleared())
        {
            ClearIndicators();
            return;
        }

        if (eruptionPrepared && Time.time - lastPhaseChange > eruptionBuildupTime + delay)
        {
            Erupt();
            delay = Random.Range(0, maxWaitTime);
        }
        else if (!eruptionPrepared && Time.time - lastPhaseChange > eruptionDormantTime)
        {
            PrepareEruption();
        }
    }

    public void Damage()
    {
        shakeStartTime = Time.time;
        --health;
        if (health < 1)
        {
            Vector3 pos = transform.position;
            Destroy(gameObject);
            GameObject floorTile = (GameObject)Instantiate(GameManager.S.floorTile, pos, Quaternion.identity);
            floorTile.GetComponent<SpriteRenderer>().sprite = GameManager.S.floorTileSprites[(int)element];
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        // this is the worst code I have ever written
        if (col.tag == "Weapon")
        {
            Weapon w = col.GetComponent<Weapon>();
            if (w != null)
            {
                if (w.element == Element.Fire && element == Element.Ice ||
                    w.element == Element.Ice && element == Element.Fire)
                {
                    Damage();
                }
            }
            else
            {
                Projectile p = col.GetComponent<Projectile>();
                if (p != null)
                {
                    if (p.element == Element.Fire && element == Element.Ice ||
                        p.element == Element.Ice && element == Element.Fire)
                    {
                        Damage();
                    }
                }
            }
        }
    }

    abstract protected void Erupt();
    abstract protected void PrepareEruption();
    abstract protected void ClearIndicators();
}
