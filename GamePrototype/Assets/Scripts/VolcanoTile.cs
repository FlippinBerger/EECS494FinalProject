using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VolcanoTile : MonoBehaviour {

    public GameObject fireballPrefab;
    public GameObject dangerIndicatorPrefab;

    public float fireballMinRadius = 2f;
    public float fireballMaxRadius = 3f;
    public float eruptionBuildupTime = 1f;
    public float eruptionDormantTime = 2f;
    public int numFireballsPerEruption = 3;

    float lastPhaseChange;
    bool eruptionPrepared = false;
    Queue<GameObject> dangerIndicators = new Queue<GameObject>();

	// Use this for initialization
	void Start () {
        lastPhaseChange = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	    if (eruptionPrepared && Time.time - lastPhaseChange > eruptionBuildupTime)
        {
            Erupt();
        }
        else if (!eruptionPrepared && Time.time - lastPhaseChange > eruptionDormantTime)
        {
            PrepareEruption();
        }
	}

    void PrepareEruption()
    {
        for (int i = 0; i < numFireballsPerEruption; ++i)
        {
            // choose random point in radius, spawn danger indicator, spawn fireball
            float dist = Random.Range(fireballMinRadius, fireballMaxRadius);

            Vector3 pos = Vector3.right * dist;
            float dir = Random.Range(0, 360);
            pos = Quaternion.Euler(0, 0, dir) * pos;

            GameObject dangerGO = (GameObject)Instantiate(dangerIndicatorPrefab, transform.position + pos, Quaternion.identity);
            dangerIndicators.Enqueue(dangerGO);
        }
        
        transform.GetComponent<SpriteRenderer>().sprite = GameManager.S.volcanoSprites[1];
        lastPhaseChange = Time.time;
        eruptionPrepared = true;
    }

    void Erupt()
    {
        for (int i = 0; i < numFireballsPerEruption; ++i)
        {
            GameObject dangerIndicator = dangerIndicators.Dequeue();
            GameObject fireballGO = (GameObject)Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            fireballGO.GetComponent<Fireball>().SetDestination(dangerIndicator.transform.position);
            fireballGO.GetComponent<Fireball>().SetAssociatedDangerIndicator(dangerIndicator);
        }

        transform.GetComponent<SpriteRenderer>().sprite = GameManager.S.volcanoSprites[0];
        lastPhaseChange = Time.time;
        eruptionPrepared = false;
    }
}
