using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeController : MonoBehaviour
{
    float time;
    float duration;

    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
        duration = transform.GetChild(0).GetComponent<ParticleSystem>().main.duration;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - time > duration / 2)
        {
            float alpha = (Time.time - time - duration / 2) / (duration / 2) * 255;
            Color color = transform.GetChild(0).GetComponent<Renderer>().material.color;
            color.a = alpha;
            transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", color);
            color = transform.GetChild(1).GetComponent<Renderer>().material.color;
            color.a = alpha;
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }
}
