using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    GameObject c_object;
    bool constructing;
    float time;

    public void startConstruction()
    {
        if (transform.Find("Construction") == null)
            return;
        c_object = transform.Find("Construction").gameObject;
        c_object.SetActive(true);
        InstructionQueue queue;
        if (gameObject.TryGetComponent<InstructionQueue>(out queue))
            queue.setActive(false);
        Attack attack;
        if (transform.GetChild(3).TryGetComponent<Attack>(out attack))
            attack.setActive(false);
        gameObject.GetComponent<Animator>().enabled = true;
        constructing = false;
        time = Time.time;
    }

    public void complete()
    {
        InstructionQueue queue;
        if (gameObject.TryGetComponent<InstructionQueue>(out queue))
            queue.setActive(false);
        Attack attack;
        if (transform.GetChild(3).TryGetComponent<Attack>(out attack))
            attack.setActive(true);
        Destroy(c_object);
        gameObject.GetComponent<Animator>().enabled = false;
        constructing = true;
    }

    public bool IsConstructing()
    {
        return constructing;
    }

    public float getStartTime()
    {
        return time;
    }
}
