using UnityEngine;
using System.Collections;

abstract public class HazardTile : MonoBehaviour
{
    public GameObject hazardPrefab;
    public GameObject dangerIndicatorPrefab;

    public float hazardMinRadius = 2f;
    public float hazardMaxRadius = 3f;
    public float eruptionBuildupTime = 1f;
    public float eruptionDormantTime = 2f;
    public int numHazardsPerEruption = 3;

    protected float lastPhaseChange;
    protected bool eruptionPrepared = false;

    // Use this for initialization
    void Start()
    {
        lastPhaseChange = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // if (transform.parent == null || !transform.parent.GetComponent<Room>().currentRoom) return;

        if (eruptionPrepared && Time.time - lastPhaseChange > eruptionBuildupTime)
        {
            Erupt();
        }
        else if (!eruptionPrepared && Time.time - lastPhaseChange > eruptionDormantTime)
        {
            PrepareEruption();
        }
    }

    abstract protected void Erupt();

    abstract protected void PrepareEruption();
}
