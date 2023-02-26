using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : ProjectileMovement
{
    [SerializeField] float speed = 100f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!set)
            return;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    protected override void setDirection()
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 acc = new Vector3(0, 0, 0);
        acc.x = Random.Range(accuracy - 1, 1 - accuracy) * 5;
        acc.y = Random.Range(accuracy - 1, 1 - accuracy) * 5;
        transform.rotation = rotation;
        transform.eulerAngles += acc;
    }
}
