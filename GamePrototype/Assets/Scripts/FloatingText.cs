using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {

    public float lifetime = 1f;
    public float speed = 1f;

    float spawntime;

    void Start()
    {
        spawntime = Time.time;
    }
    
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos.y += speed * Time.deltaTime;
        transform.position = pos;
        // transform.rotation = Quaternion.identity;
        
        if (Time.time - spawntime > lifetime)
        {
            Destroy(gameObject);
        }
	}
}
