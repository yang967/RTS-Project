using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBulletMovement : ProjectileMovement
{
    float a, b, c;
    Vector3 start;

    // Start is called before the first frame update
    void Start()
    {
        c = transform.position.y;
        start = transform.position;
        float x_a = distanceFromStart(target);
        float y_a = target.y;
        float x_b = x_a / 2.0f;
        float y_b = Mathf.Max(start.y, target.y) + x_b / 10;
        float y_c = (y_a - c) * x_b * x_b - (y_b - c) * x_a * x_a;
        float x_c = x_a * x_b * x_b - x_b * x_a * x_a;
        b = y_c / x_c;
        a = (y_a - c - b * x_a) / (x_a * x_a);
    }


    void FixedUpdate()
    {
        if (!set)
            return;
        float half = distanceFromStart(target) / 2.0f;
        Vector3 nextPos = transform.position + transform.forward * 100 * Time.deltaTime;
        float height = getHeight(nextPos);
        nextPos.y = height;
        transform.GetChild(0).LookAt(nextPos);
        transform.position = new Vector3(nextPos.x, height, nextPos.z);
    }

    float getHeight(Vector3 pos)
    {
        float distance = distanceFromStart(pos);
        return a * distance * distance + b * distance + c;
    }

    float distanceFromStart(Vector3 pos)
    {
        return Mathf.Sqrt((pos.x - start.x) * (pos.x - start.x) + (pos.z - start.z) * (pos.z - start.z));
    }
}
