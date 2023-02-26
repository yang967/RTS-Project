using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InRange : MonoBehaviour
{
    [SerializeField] Attack atk;

    public void setRadius(float radius)
    {
        GetComponent<CapsuleCollider>().radius = radius * 3 / 4;
        float height = GetComponent<CapsuleCollider>().height;
        GetComponent<CapsuleCollider>().height = radius > 40 ? height : radius * 2;
    }

    private void OnTriggerStay(Collider other)
    {
        if (atk.getTarget() == null || !other.gameObject.Equals(atk.getTarget()))
            return;
        atk.setInRange();
    }
}
