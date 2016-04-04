using UnityEngine;
using System.Collections;

public class Snowflake : Hazard {

    public float lifetime = 1.5f;

    float spawntime;
    Vector3 orbitPoint = Vector3.zero;
    float speed = 0;
    float angle = 0;
    float radius = 0;

    void Start()
    {
        spawntime = Time.time;
    }

    public void SetOrbitPoint(Vector3 orbit)
    {
        orbitPoint = orbit;
        Vector3 relative = transform.position - orbit;
        angle = Vector3.Angle(Vector3.right, relative);
        radius = relative.magnitude;
        speed = (2 * Mathf.PI) / radius;
    }
  
	// Update is called once per frame
	void Update () {
        if (Time.time - spawntime > lifetime)
        {
            Destroy(gameObject);
        }

        angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
        Vector3 pos = Vector3.zero;
        pos.x = Mathf.Cos(angle) * radius;
        pos.y = Mathf.Sin(angle) * radius;
        transform.position = orbitPoint + pos;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Enemy")
        {
            Actor actor = col.GetComponent<Actor>();
            actor.Freeze(34f);
        }
    }
}
