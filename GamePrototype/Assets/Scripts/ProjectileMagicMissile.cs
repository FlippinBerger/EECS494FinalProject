using UnityEngine;
using System.Collections;

public class ProjectileMagicMissile : Projectile {

    public float lockOnDelay = 0.33f;
    public float torque = 2;
    // upgrades
    [HideInInspector]
    public bool homing = false;

    public Room currentRoom;

    GameObject target;

    void Start()
    {
        if (homing)
        {
            StartCoroutine(LockOnToTarget());
        }
    }

    IEnumerator LockOnToTarget()
    {
        yield return new WaitForSeconds(lockOnDelay);
        target = currentRoom.GetClosestEnemyTo(this.gameObject);
    }

    protected override void Update()
    {
        if (homing)
        {
            if (target != null)
            {
                Vector2 relative = target.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward, relative);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, torque);
            }
            else
            {
                target = currentRoom.GetClosestEnemyTo(this.gameObject);
            }
        }
        rigid.velocity = transform.up * missileSpeed;
        base.Update();
    }
}
