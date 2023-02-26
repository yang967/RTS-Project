using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMissleMovement : ProjectileMovement
{
    Quaternion rotation;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!set)
            return;
        setDirection();
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rotation, Time.deltaTime * 50);
        transform.position += transform.forward * 100f * Time.deltaTime;
    }

    protected override void setDirection()
    {
        Vector3 direction = (target - transform.position).normalized;
        rotation = Quaternion.LookRotation(direction);
    }
}
