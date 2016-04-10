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
    public int numHazardsPerEruption = 3;
    public Element element;

    protected float lastPhaseChange;
    protected bool eruptionPrepared = false;

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
        lastPhaseChange = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null || !transform.parent.GetComponent<Room>().currentRoom) return;

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

        if (eruptionPrepared && Time.time - lastPhaseChange > eruptionBuildupTime)
        {
            Erupt();
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

    abstract protected void Erupt();

    abstract protected void PrepareEruption();
}
