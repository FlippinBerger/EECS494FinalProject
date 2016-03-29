using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {

    public Enemy parentEnemy;
    public int damage;
    public float knockbackVelocity;
    public float knockbackDuration;
    public GameObject target;
}
