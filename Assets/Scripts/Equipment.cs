using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Equipment
{
    private string Name;
    private int Damage;
    private int ArmorPeneration;
    private int Armor;
    private float Speed;
    private float Range;
    private float ViewRange;
    private float FireRate;
    private string Type;
    private int[] AntiType;
    protected string Unit;
    private int index;
    private int pellet;
    private int burst;
    private float accuracy;
    private AbilityData[] abilities;
    private bool blank;

    public Equipment()
    {
        Name = "";
        Damage = 0;
        ArmorPeneration = 0;
        Armor = 0;
        Speed = 0;
        Range = 0;
        ViewRange = 0;
        FireRate = 0;
        Type = "";
        Unit = "";
        AntiType = new int[] { 0, 0, 0, 0 };
        index = 0;
        burst = 0;
        accuracy = 0;
        abilities = new AbilityData[0];
        pellet = 0;
        blank = true;
    }

    public Equipment(string name) : this()
    {
        Name = name;
    }

    public Equipment(string name, int damage, int armorpenetration, float firerate, float accuracy, float range, int armor, float speed, float viewrange, string type, string unit, int index, int[] antitype, int pellet, int burst, AbilityData[] abilities)
    {
        Name = name;
        Damage = damage;
        ArmorPeneration = armorpenetration;
        Armor = armor;
        Speed = speed;
        Type = type;
        Unit = unit;
        FireRate = firerate;
        this.accuracy = accuracy;
        Range = range;
        ViewRange = viewrange;
        AntiType = new int[4];
        antitype.CopyTo(AntiType, 0);
        this.index = index;
        this.pellet = pellet;
        this.burst = burst;
        this.abilities = new AbilityData[abilities.Length];
        abilities.CopyTo(this.abilities, 0);
        blank = false;
    }

    public Equipment(Equipment equipment)
    {
        Name = equipment.getName();
        Damage = equipment.getDamage();
        ArmorPeneration = equipment.getArmorPenetration();
        Armor = equipment.getArmor();
        Speed = equipment.getSpeed();
        Type = equipment.getType();
        Unit = equipment.getUnit();
        FireRate = equipment.getFireRate();
        Range = equipment.getRange();
        ViewRange = equipment.getViewRange();
        AntiType = new int[4];
        equipment.getAntiType().CopyTo(AntiType, 0);
        index = equipment.getIndex();
        pellet = equipment.getPellet();
        burst = equipment.burstFire();
        accuracy = equipment.getAccuracy();
        abilities = new AbilityData[equipment.getAbility().Length];
        equipment.getAbility().CopyTo(abilities, 0);
        blank = equipment.isBlank();
    }

    public AbilityData[] getAbility()
    {
        return abilities;
    }

    public AbilityData getAbility(int index)
    {
        return abilities[index];
    }

    public int burstFire()
    {
        return burst;
    }

    public float getAccuracy()
    {
        return accuracy;
    }

    public string getName()
    {
        return Name;
    }

    public int getDamage()
    {
        return Damage;
    }

    public int getArmorPenetration()
    {
        return ArmorPeneration;
    }

    public int getArmor()
    {
        return Armor;
    }

    public float getSpeed()
    {
        return Speed;
    }

    public string getType()
    {
        return Type;
    }

    public string getUnit()
    {
        return Unit;
    }

    public float getFireRate()
    {
        return FireRate;
    }

    public float getRange()
    {
        return Range;
    }

    public float getViewRange()
    {
        return ViewRange;
    }

    public int[] getAntiType()
    {
        return AntiType;
    }

    public void setIndex(int i)
    {
        if (!Name.Equals(""))
            return;
        index = i;
    }

    public int getIndex()
    {
        return index;
    }

    public int getPellet()
    {
        return pellet;
    }

    public bool isBlank()
    {
        return blank;
    }
}
