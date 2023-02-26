using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryControl : MovementControl
{
    Quaternion rotation;

    public override void MoveTo(Vector3 target, bool considerUnit)
    {
        base.MoveTo(target, considerUnit);
        GroundUnitCollision collision = gameObject.GetComponent<GroundUnitCollision>();
        collision.setDetect(true);
        collision.setClose(true);
        collision.setTarget(target);
        generatePath(target);
        finalTarget = target;
        lastTarget = finalTarget;
        calculate();
    }

    protected override void calculate()
    {
        if (Mathf.Abs(target.x - transform.position.x) <= 1 && Mathf.Abs(target.z - transform.position.z) <= 1)
        {
            if(path.Count > 0)
            {
                target = path[path.Count - 1];
                target.y = Terrain.activeTerrain.SampleHeight(target);
                path.RemoveAt(path.Count - 1);
            }
            else
            {
                finalTarget = transform.position;
                target = transform.position;
                target.y = Terrain.activeTerrain.SampleHeight(target);
                moving = false;
                turning = false;
            }
            return;
        }
        rotation = Quaternion.LookRotation(target - transform.position);
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        distance = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.z - transform.position.z, 2));
    }

    protected override void Turn()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.3f);
    }
}
