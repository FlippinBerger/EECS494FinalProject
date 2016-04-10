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
        rigid.velocity = transform.up * missileSpeed;
        if (homing && target != null)
        {
            Vector3 relativePos = target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(relativePos);
            Quaternion rotation = Quaternion.Lerp(transform.rotation, targetRotation, torque * Time.deltaTime);

            transform.rotation = rotation;
        }
        base.Update();
    }
}
