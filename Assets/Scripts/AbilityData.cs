using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityData
{
    string type;
    string name;
    int group;
    float radius;
    float distance;
    float duration;
    int damage;
    int armorpenetration;
    string objName;
    int[] antitype;
    int charge;
    float cd;
    Effect effect;

    public AbilityData()
    {
        type = "";
        name = "";
        group = -1;
        damage = 0;
        distance = 0;
        armorpenetration = 0;
        antitype = new int[] { -1, -1, -1, -1 };
        effect = new Effect();
    }

    public AbilityData(AbilityData ability)
    {
        type = ability.getAbilityType();
        name = ability.getName();
        group = ability.getGroup();
        radius = ability.getRadius();
        distance = ability.getDistance();
        duration = ability.getDuration();
        damage = ability.getDamage();
        armorpenetration = ability.getArmorPenetration();
        antitype = new int[4];
        ability.getAntiType().CopyTo(antitype, 0);
        effect = new Effect(ability.getEffect());
        objName = ability.getObjectName();
    }

    public AbilityData(string type, string name, float radius, float distance, float duration, int damage, int armorpenetration, int charge, float cd, int[] antitype, Effect effect)
    {
        group = -1;
        this.type = type;
        this.name = name;
        this.radius = radius;
        this.distance = distance;
        this.duration = duration;
        this.damage = damage;
        this.armorpenetration = armorpenetration;
        this.antitype = new int[4];
        antitype.CopyTo(this.antitype, 0);
        this.effect = new Effect(effect);
        objName = "";
        this.charge = charge;
        this.cd = cd;
    }

    public AbilityData(string type, string name, float radius, float distance, float duration, int damage, int armorpenetration, string objName, int charge, int cd, int[] antitype, Effect effect)
    {
        group = -1;
        this.type = type;
        this.name = name;
        this.radius = radius;
        this.distance = distance;
        this.duration = duration;
        this.damage = damage;
        this.armorpenetration = armorpenetration;
        this.antitype = new int[4];
        antitype.CopyTo(this.antitype, 0);
        this.effect = new Effect(effect);
        this.objName = objName;
        this.charge = charge;
        this.cd = cd;
    }

    public string getAbilityType()
    {
        return type;
    }

    public string getName()
    {
        return name;
    }

    public int getGroup()
    {
        return group;
    }

    public float getRadius()
    {
        return radius;
    }

    public float getDistance()
    {
        return distance;
    }

    public float getDuration()
    {
        return duration;
    }

    public int getDamage()
    {
        return damage;
    }

    public int getArmorPenetration()
    {
        return armorpenetration;
    }

    public string getObjectName()
    {
        return objName;
    }

    public int[] getAntiType()
    {
        return antitype;
    }

    public Effect getEffect()
    {
        return effect;
    }

    public int getCharge()
    {
        return charge;
    }

    public float getCD()
    {
        return cd;
    }
}
