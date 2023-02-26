using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonate : MonoBehaviour
{
    List<GameObject> nearbyUnit;
    Effect effect = null;
    // Start is called before the first frame update
    void Start()
    {
        nearbyUnit = new List<GameObject>();
    }

    public void setEffect(Effect e)
    {
        if (effect != null)
            return;
        effect = new Effect(e);
    }

    private void OnTriggerEnter(Collider other)
    {
        try {
            if ((other.gameObject.layer == 3 || other.gameObject.layer == 8) && other.gameObject.GetComponentInChildren<MeshRenderer>().enabled)
                nearbyUnit.Add(other.gameObject);
        } catch(System.NullReferenceException) { return; }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 3 && other.gameObject.layer != 8)
            return;
        try
        {
            nearbyUnit.Remove(other.gameObject);
        } catch(System.Exception) { }
    }

    public void detonate(int damage, int armorpenetration, int[] damageType)
    {
        foreach (GameObject gameobj in nearbyUnit)
        {
            try
            {
                if (gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                    continue;
                gameobj.transform.GetChild(0).GetComponent<UnitLoad>().setHitPoint(damage * 10 + 2, armorpenetration, damageType);
            }
            catch (MissingReferenceException)
            {
                continue;
            }
        }
        if(effect != null && effect.getEffectType() != -1)
        {
            string name = effect.getName() + "Field";
            GameObject obj = Resources.Load("EffectField") as GameObject;
            Vector3 pos = transform.position;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
            GameObject effectfield = Instantiate(obj, pos, Quaternion.identity);
            effectfield.GetComponent<EffectLoad>().setEffect(effect);
        }
    }
}
