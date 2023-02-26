using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HasObstacle
{
    static Ray ray;

    public static bool hasObstacle(float3 position, int distance, bool considerUnit)
    {
        LayerMask ignore = (1 << 3) | (1 << 2);
        Vector3 pos = new Vector3(position.x, position.y, position.z) ;
        pos.y = Terrain.activeTerrain.SampleHeight(pos) + 1;

        RaycastHit hit;
        ray.origin = pos + Vector3.back * distance;
        ray.direction = Vector3.forward;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red, 10);
        //float time = Time.realtimeSinceStartup;
        if (considerUnit && Physics.Raycast(ray, out hit, 2 * distance))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }

        ray.origin = pos + Vector3.right * distance;
        ray.direction = Vector3.left;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red, 10);
        if (considerUnit && Physics.Raycast(ray, out hit, 2 * distance))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }

        ray.origin = pos + (Vector3.forward + Vector3.right) * distance;
        ray.direction = (Vector3.back + Vector3.left).normalized;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * (distance * 2), Color.red, 10);
        if (considerUnit && Physics.Raycast(ray, out hit, 2 * distance))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }

        ray.origin = pos + (Vector3.forward + Vector3.left) * distance;
        ray.direction = (Vector3.back + Vector3.right).normalized;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * (distance * 2), Color.red, 10);
        if (considerUnit && Physics.Raycast(ray, out hit, 2 * distance))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }
        else if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
        {
            //Debug.Log(hit.collider.name);
            return true;
        }
        //Debug.Log((Time.realtimeSinceStartup - time) * 1000 + "ms");

        return false;
    }
}
