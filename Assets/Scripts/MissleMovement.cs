using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleMovement : ProjectileMovement
{
    float starttime;
    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        starttime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!set)
            return;
        //if (Mathf.Abs(transform.eulerAngles.x - rotation.x) > 0.05 && Mathf.Abs(transform.eulerAngles.y - rotation.y) > 0.05)
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rotation, Time.deltaTime * 50);
        transform.position += transform.forward * 50f * Time.deltaTime;
    }

    protected override void setDirection()
    {
        Vector3 direction = (target - transform.position).normalized;
        rotation = Quaternion.LookRotation(direction);
    }


    public override void Set()
    {
        set = false;
        transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
    }
}
