using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AbilityLoad : MonoBehaviour
{
    List<AbilityData> abilities;
    List<AStats> aStats;
    Unit unit;

    // Start is called before the first frame update
    void Awake()
    {
        unit = transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
        abilities = new List<AbilityData>(unit.getAbility());
        aStats = new List<AStats>();
        for (int i = 0; i < abilities.Count; i++)
            aStats.Add(new AStats(abilities[i].getCharge(), abilities[i].getCD()));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < aStats.Count; i++)
            aStats[i].check();
    }

    public void updateAblity()
    {
        unit = transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
        List<AbilityData> tmp = unit.getAbility();
        for (int i = 0; i < tmp.Count; i++)
        {
            if (i >= abilities.Count)
                aStats.Add(new AStats(tmp[i].getCharge(), tmp[i].getCD()));
            else if (abilities[i].getName() != tmp[i].getName())
                aStats[i] = new AStats(tmp[i].getCharge(), tmp[i].getCD());
        }
        for (int i = abilities.Count - 1; i >= tmp.Count; i--)
            aStats.RemoveAt(i);
        abilities = new List<AbilityData>(tmp);
    }

    public AbilityData get(int i)
    {
        if (i >= abilities.Count)
            return null;
        return abilities[i];
    }

    public float getCD(int i)
    {
        if (i >= abilities.Count)
            return 0;
        return aStats[i].getCD();
    }

    public int getCharge(int i)
    {
        if (i >= abilities.Count)
            return 0;
        return aStats[i].getCharge();
    }

    public bool Use(int i)
    {
        if (i >= abilities.Count)
            return false;
        return aStats[i].Use();
    }

    public bool Usable(int i)
    {
        return i < abilities.Count && aStats[i].Usable();
    }

    [System.Serializable]
    private class AStats
    {
        int total_charge;
        int charge;
        float cd;
        float time;

        public AStats(int charge, float cd)
        {
            total_charge = charge;
            this.charge = charge;
            time = Time.time;
            this.cd = cd;
        }

        public bool Use()
        {
            if (charge <= 0)
                return false;
            charge--;
            time = Time.time;
            return true;
        }

        public void check()
        {
            if (Time.time - time < cd || charge >= total_charge)
                return;
            time = Time.time;
            charge++;
        }

        public int getCharge()
        {
            return charge;
        }

        public float getCD()
        {
            if (charge >= total_charge)
                return -1;
            return (Time.time - time) / cd;
        }

        public bool Usable()
        {
            return charge > 0;
        }
    }
}