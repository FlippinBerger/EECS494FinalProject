using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VolcanoTile : HazardTile {

    Queue<GameObject> dangerIndicators = new Queue<GameObject>();

    override protected void PrepareEruption()
    {
        for (int i = 0; i < numHazardsPerEruption; ++i)
        {
            // choose random point in radius, spawn danger indicator, spawn fireball
            float dist = Random.Range(hazardMinRadius, hazardMaxRadius);

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

    override protected void Erupt()
    {
        for (int i = 0; i < numHazardsPerEruption; ++i)
        {
            GameObject dangerIndicator = dangerIndicators.Dequeue();
            GameObject fireballGO = (GameObject)Instantiate(hazardPrefab, transform.position, Quaternion.identity);
            fireballGO.GetComponent<Fireball>().SetDestination(dangerIndicator.transform.position);
            fireballGO.GetComponent<Fireball>().SetAssociatedDangerIndicator(dangerIndicator);
        }

        transform.GetComponent<SpriteRenderer>().sprite = GameManager.S.volcanoSprites[0];
        lastPhaseChange = Time.time;
        eruptionPrepared = false;
    }
}
