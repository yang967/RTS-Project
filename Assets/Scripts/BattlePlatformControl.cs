using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlatformControl : VehicleControl
{
    protected override void Turn()
    {
        angle = 0;
        turning = false;
    }

    protected override void Move()
    {
        target.y = Terrain.activeTerrain.SampleHeight(target) + transform.position.y - Terrain.activeTerrain.SampleHeight(transform.position);
        float height = transform.position.y;
        Vector3 position = Vector3.MoveTowards(transform.position, target, Movingspeed * Time.deltaTime);
        transform.position = position;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
