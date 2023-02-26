using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    protected bool set = false;
    //protected GameObject target;
    protected Vector3 target;
    protected bool nonTarget;
    protected float accuracy;

    public virtual void setTarget(GameObject target, float accuracy)
    {
        Vector3 vec = target.transform.position;
        this.target = new Vector3(vec.x, vec.y, vec.z);
        set = true;
        this.accuracy = accuracy;
        setDirection();
    }

    public virtual void setTarget(Vector3 target, float accuracy)
    {
        this.target = new Vector3(target.x, target.y, target.z);
        set = true;
        this.accuracy = accuracy;
        setDirection();
    }

    public virtual void Set()
    {
        set = false;
    }

    protected virtual void setDirection() { }

    public Vector3 getTarget()
    {
        return target;
    }
}
