using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnowCloudTile : HazardTile {

    GameObject dangerIndicatorGO;

    override protected void PrepareEruption()
    {
        dangerIndicatorGO = (GameObject)Instantiate(dangerIndicatorPrefab, transform.position, Quaternion.identity);
        dangerIndicatorGO.transform.localScale = new Vector3(hazardMaxRadius * 2, hazardMaxRadius * 2, 1);
        dangerIndicatorGO.transform.parent = transform;
        lastPhaseChange = Time.time;
        eruptionPrepared = true;
    }

    override protected void Erupt()
    {
		AudioSource audio = GetComponent<AudioSource> ();
		audio.Play ();

        Destroy(dangerIndicatorGO);
        for (int i = 0; i < numHazardsPerEruption; ++i)
        {
            // choose random point in radius, spawn snowflake
            float dist = Random.Range(hazardMinRadius, hazardMaxRadius);

            Vector3 pos = Vector3.right * dist;
            float dir = Random.Range(0, 360);
            pos = Quaternion.Euler(0, 0, dir) * pos;

            GameObject snowflakeGO = (GameObject)Instantiate(hazardPrefab, transform.position + pos, Quaternion.identity);
            snowflakeGO.GetComponent<Snowflake>().SetOrbitPoint(transform.position);
        }
        lastPhaseChange = Time.time;
        eruptionPrepared = false;
		Invoke ("quiet", 3.0f);
    }

	protected void quiet(){
		AudioSource audio = GetComponent<AudioSource> ();
		audio.Stop ();
	}

    protected override void ClearIndicators()
    {
        if (dangerIndicatorGO != null)
        {
            Destroy(dangerIndicatorGO);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
        if (col.tag == "Player" || col.tag == "Enemy")
        {
            Actor actor = col.GetComponent<Actor>();
            actor.Freeze(100);
            Vector2 dir = col.transform.position - transform.position;
            actor.Knockback(2f, dir, 0.5f);
        }
    }
}
