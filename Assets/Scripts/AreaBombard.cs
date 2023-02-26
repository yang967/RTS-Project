using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBombard : AbilityAbs
{
    public override void execute()
    {
        currentTime = Time.time;
        Vector3 position;
        float division = 2 * Mathf.PI / ObjectOut.Length;
        float angle = 0;
        float distance = radius / 2.0f;
        for (int i = 0; i < ObjectOut.Length; i++)
        {
            angle += division;
            position = new Vector3(Mathf.Cos(angle) * distance + targetVector.x, 0, Mathf.Sin(angle) * distance + targetVector.z);
            position.y = Terrain.activeTerrain.SampleHeight(position);
            position.y += 1.5f;
            GameObject missle = Instantiate(obj, ObjectOut[i].transform.position, Quaternion.identity);
            missle.GetComponent<Bullet>().Attack(position, ObjectOut[i], damage, armorpenetration, 1, antitype);
            missle.GetComponent<Bullet>().setEffect(effect);
        }
    }
}
