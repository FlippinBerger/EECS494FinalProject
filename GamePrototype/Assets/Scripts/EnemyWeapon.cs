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
    [HideInInspector]
    public bool reflected = false;

    protected virtual void Start()
    {

    }

    protected virtual void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Actor actor = col.GetComponent<Actor>();
            Vector2 direction = actor.transform.position - parentEnemy.transform.position;
            damage += (parentEnemy.attackScalingAmount * (GameManager.S.round - 1) / 2); // damage upgrade every two levels
            actor.Hit(new AttackHitInfo(damage, knockbackVelocity, knockbackDuration, parentEnemy.element, parentEnemy.elementalLevel, parentEnemy.gameObject), direction);
        }
    }
    
}
