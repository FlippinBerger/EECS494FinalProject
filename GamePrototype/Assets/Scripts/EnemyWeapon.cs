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
    
}
