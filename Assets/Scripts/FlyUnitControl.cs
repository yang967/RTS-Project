using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlyUnitControl : MovementControl
{
    float lastDistance;
    protected override void Start()
    {
        base.Start();
        lastDistance = 1;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (disabled)
            return;
        if(Time.time - time > 3 && lastDistance > distance)
        {
            turning = false;
            moving = false;
            close = true;
        }
        lastDistance = distance;
    }

    public override void MoveTo(Vector3 target, bool canceltarget)
    {
        base.MoveTo(target, canceltarget);
        finalTarget = target;
        finalTarget.y = Terrain.activeTerrain.SampleHeight(finalTarget);
        this.target = finalTarget;
        lastTarget = finalTarget;
        lastDistance = distance + 1;
    }

    protected override void calculate()
    {
        if(Mathf.Abs(target.x - transform.position.x) <= 1 && Mathf.Abs(target.z - transform.position.z) <= 1)
        {
            target = transform.position;
            return;
        }
        Quaternion rotation = Quaternion.LookRotation(target - transform.position);
        angle = rotation.eulerAngles.y - transform.eulerAngles.y;
        if (angle > 185)
            angle -= 360;
        if (angle < -185)
            angle += 360;
        distance = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.z - transform.position.z, 2));
    }

    protected override void Turn()
    {
        base.Turn();
        if (Mathf.Abs(angle) > 0.2)
            animator.SetBool("Forward", true);
        else
            animator.SetBool("Forward", false);
    }

    protected override void Move()
    {
        if (turning && close)
            return;
        if (Mathf.Abs(distance) < 1)
        {
            animator.SetBool("Forward", false);
            animator.SetBool("Backward", false);
            moving = false;
            close = false;
        }
        else if (distance >= 1)
        {
            animator.SetBool("Forward", true);
            moving = true;
            Forward();
        }
        else if (distance <= -1)
        {
            animator.SetBool("Backward", true);
            moving = true;
            Backward();
        }
    }

    void Forward()
    {
        transform.position += transform.forward * Time.deltaTime * Movingspeed;
    }

    void Backward()
    {
        transform.position -= transform.forward * Time.deltaTime * Movingspeed;
    }
}


