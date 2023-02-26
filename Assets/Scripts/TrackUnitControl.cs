using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackUnitControl : MovementControl
{

    public override void MoveTo(Vector3 target, bool considerUnit)
    {
        base.MoveTo(target, considerUnit);
        GroundUnitCollision collision = gameObject.GetComponent<GroundUnitCollision>();
        collision.setDetect(true);
        collision.setClose(true);
        collision.setTarget(target);
        generatePath(target);
        finalTarget = target;
        finalTarget.y = Terrain.activeTerrain.SampleHeight(finalTarget);
        lastTarget = finalTarget;
        calculate();
    }

    protected override void calculate()
    {
        if (moving || turning)
            return;
        if (Mathf.Abs(target.x - transform.position.x) <= 1 && Mathf.Abs(target.z - transform.position.z) <= 1)
        {
            if (path.Count > 0)
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
                return;
            }
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
        if (moving)
            return;
        base.Turn();
    }

    protected override void Move()
    {
        if (turning)
            return;
        base.Move();
    }
}
