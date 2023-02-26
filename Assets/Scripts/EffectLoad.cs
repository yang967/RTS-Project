using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLoad : MonoBehaviour
{
    Effect effect;
    bool set = false;
    List<GameObject> affected = new List<GameObject>();
    float time;
    float total;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!set)
            return;
        if (Time.time - total >= effect.getTime())
        {
            set = false;
            if(transform.GetChild(0).GetComponent<ParticleSystem>() != null)
                transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            else
                for(int i = 0; i < transform.GetChild(0).childCount; i++)
                    transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Stop();
            return;
        }
        UnitLoad uload;
        if(effect.getEffectType() == 0)
        {
            if (Time.time - time < 1)
                return;
            foreach (GameObject gameobj in affected)
            {
                try
                {
                    uload = gameobj.transform.GetChild(0).GetComponent<UnitLoad>();
                    if (effect.getAntiType()[uload.OutputUnit().getUnitType() / 10] == -1)
                        continue;
                    uload.setHitPoint(effect.getValue() * 10 + 2, uload.OutputUnit().getArmor(), effect.getAntiType());
                }
                catch (MissingReferenceException)
                {
                    continue;
                }
            }
            time = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer != 3 && other.gameObject.layer != 8))
            return;
        affected.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (effect == null)
            return;
        if (effect.getEffectType() == 0)
            affected.Remove(other.gameObject);
    }

    public void setEffect(Effect e)
    {
        this.effect = new Effect(e);
        gameObject.GetComponent<CapsuleCollider>().radius = effect.getRange();
        GameObject gameobj = Resources.Load(effect.getInGameEffectName()) as GameObject;
        Instantiate(gameobj, transform);
        set = true;
        time = Time.time;
        total = Time.time;
        Destroy(gameObject, effect.getTime() + 2);
    }
}
