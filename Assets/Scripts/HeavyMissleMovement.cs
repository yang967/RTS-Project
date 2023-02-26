using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyMissleMovement : ProjectileMovement
{
    float velocity;
    Quaternion rotation;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        velocity = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!set)
            return;
        if(Mathf.Abs(transform.eulerAngles.x - rotation.x) > 0.05 && Mathf.Abs(transform.eulerAngles.y - rotation.y) > 0.05 && Time.time - time >= 0.5f)
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rotation, Time.deltaTime * 5);
        transform.position += transform.forward * Mathf.Min(velocity += 0.2f, 30) * Time.deltaTime;
    }

    protected override void setDirection()
    {
        time = Time.time;
        Vector3 direction = (target - transform.position).normalized;
        rotation = Quaternion.LookRotation(direction);
    }

    public override void Set()
    {
        set = false;
        transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
    }

}
