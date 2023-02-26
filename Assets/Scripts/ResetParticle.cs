using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetParticle : MonoBehaviour
{
    public void ResetEffect(int i)
    {
        Transform t1 = transform.Find("Destroy").GetChild(i);
        foreach (Transform t2 in t1)
        {
            if (t2.childCount > 0)
                foreach (Transform t3 in t2)
                {
                    t3.GetComponent<ParticleSystem>().Simulate(0, true, true);
                    t3.GetComponent<ParticleSystem>().Play();
                }
        }
    }

}
