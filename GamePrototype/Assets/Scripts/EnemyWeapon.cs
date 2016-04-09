using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {
    [HideInInspector]
    public Enemy parentEnemy;
    [HideInInspector]
    public int damage;
    [HideInInspector]
    public float knockbackVelocity;
    [HideInInspector]
    public float knockbackDuration;

    [HideInInspector]
    public GameObject target;

    protected virtual void Start()
    {

    }

    protected virtual void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Player p = col.GetComponent<Player>();
            Vector2 direction = p.transform.position - parentEnemy.transform.position;
            p.Hit(new AttackHitInfo(damage, knockbackVelocity, knockbackDuration, parentEnemy.element, parentEnemy.gameObject), direction);
        }
    }
    
}
