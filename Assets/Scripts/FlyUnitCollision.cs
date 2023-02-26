using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyUnitCollision : MonoBehaviour
{
    Collider c;
    Collider thisCollider;
    bool active;

    void Start()
    {
        active = true;
        c = new Collider();
        thisCollider = transform.GetComponent<SphereCollider>();
    }

    public void setActive(bool active)
    {
        this.active = active;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active)
            return;
        if (other.GetType() != typeof(SphereCollider) || !other.name.Equals("detect") || !other.transform.parent.GetComponentInChildren<MeshRenderer>().enabled)
            return;
        if (transform.parent.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isStatic() || transform.parent.GetChild(0).name.Equals("BattlePlatform"))
            return;
        try
        {
            Unit o = other.gameObject.transform.parent.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
            if (!o.isFlyUnit())
                return;
        }
        catch(System.Exception)
        {
            return;
        }
        Vector3 move = Vector3.MoveTowards(transform.parent.position, other.transform.position, -5 * Time.deltaTime);
        if (transform.parent.GetComponent<MovementControl>().isIdle())
            transform.parent.GetComponent<MovementControl>().cancelTarget();
        move.y = transform.parent.position.y;
        transform.parent.position = move;
    }
}
