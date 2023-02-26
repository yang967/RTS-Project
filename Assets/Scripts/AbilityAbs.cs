using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAbs : MonoBehaviour
{
    protected float delay = 0;
    protected float duration = 0;
    protected int type = -1;
    protected float currentTime = 0;
    protected GameObject[] ObjectOut = null;
    protected GameObject obj = null;
    protected GameObject target = null;
    protected Vector3 targetVector = new Vector3(65535, 65535, 65535);
    protected int damage = 0;
    protected int armorpenetration = 0;
    protected Effect effect = null;
    protected int[] antitype = null;
    protected int group = -1;
    protected float radius;
    protected float distance = 0;
    protected bool isSet = false;

    public void setTargetVector(Vector3 vector)
    {
        targetVector = new Vector3(vector.x, vector.y, vector.z);
    }

    public Vector3 getTargetVector()
    {
        return targetVector;
    }

    public void set(int group, GameObject[] objOut, GameObject obj, GameObject target, float radius, float distance, float duration, int damage, int armorpenetration, int[] antitype, Effect effect)
    {
        this.group = group;
        ObjectOut = new GameObject[objOut.Length];
        objOut.CopyTo(ObjectOut, 0);
        this.obj = obj;
        this.target = target;
        this.radius = radius;
        this.distance = distance;
        this.duration = duration;
        this.damage = damage;
        this.armorpenetration = armorpenetration;
        this.antitype = new int[4];
        antitype.CopyTo(this.antitype, 0);
        this.effect = new Effect(effect);
        isSet = true;
    }

    public virtual void execute() { }
}
