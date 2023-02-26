using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRange : MonoBehaviour
{
    Attack attack;

    // Start is called before the first frame update
    void Start()
    {
        attack = transform.parent.GetChild(3).GetComponent<Attack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attack == null)
            return;
        attack.AddTarget(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (attack == null)
            return;
        attack.RemoveTarget(other.gameObject);
    }
}
