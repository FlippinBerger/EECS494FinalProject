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
        // set attributes of enemy weapon based on enemy
        this.damage = this.parentEnemy.damage;
        this.knockbackVelocity = this.parentEnemy.knockbackVelocity;
        this.knockbackDuration = this.parentEnemy.knockbackDuration;
    }
}
