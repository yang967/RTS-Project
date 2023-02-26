using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

[System.Serializable]
public class Unit
{
    private int Armor;
    private int Damage;
    private int Troop;
    private int TroopType;
    private bool TroopAttack;
    private bool TroopOut;
    private string Type;
    private int Group;
    private float Speed;
    private int ArmorPenetration;
    private List<Equipment> equipment;
    private float Range;
    private float ViewRange;
    private float[] Position;
    private float[] Rotation;
    private Player player;
    private float FireRate;
    private long ID;
    private int HitPoint;
    private int[] AntiType;
    private int[] initialAntiType;
    private bool TrackUnit;
    private float TurningSpeed;
    private int MaxHitPoint;
    private int UnitType;
    private string[] troops;
    private int currentTroop;
    private float height;
    private List<AbilityData> Ability;
    private int burst;
    private bool sta;
    private float accuracy;
    private int pellet;
    private int cost;
    private float produce_time;
    private int size;
    private int shield;
    private int max_shield;
    private float normal_range;

    public Unit(GameObject go)
    {
        Armor = 0;
        Damage = 0;
        Troop = 0;
        TroopType = -1;
        TroopAttack = false;
        TroopOut = false;
        TrackUnit = false;
        Type = "";
        Group = -1;
        Speed = 1;
        ArmorPenetration = 0;
        ID = 0;
        HitPoint = 100;
        MaxHitPoint = HitPoint;
        equipment = null;
        Range = 0;
        ViewRange = 0;
        normal_range = 0;
        AntiType = new int[] { 0, 0, 0, 0 };
        initialAntiType = new int[] { 0, 0, 0, 0 };
        TurningSpeed = 1;
        troops = new string[0];
        currentTroop = 0;
        height = 0;
        burst = 0;
        Ability = new List<AbilityData>();
        sta = false;
        accuracy = 0;
        pellet = 0;
        cost = 0;
        produce_time = 0;
        size = 0;
        shield = 0;
        max_shield = -2;
    }
    public Unit(int hitpoint, int shield, int max_shield, int size, int armor, int damage, int troop, bool trackunit, string type, int group, float speed, float turningspeed, int armorpenetration, float range, float viewRange, float firerate, float accuracy, int pellet, int burst, int unittype, int[] antitype, float height, GameObject go, int equipnum, bool sta, int cost, float produce_time, List<Equipment> equip, Player p)
    {
        Armor = armor;
        Damage = damage;
        Troop = troop;
        TroopType = -1;
        TroopAttack = false;
        TroopOut = false;
        Type = type;
        Group = group;
        Speed = speed;
        ArmorPenetration = armorpenetration;
        equipment = new List<Equipment>();
        equipment.Capacity = equipnum;
        initializeEquipment();
        setEquipment(equip);
        Range = range;
        normal_range = range;
        ViewRange = viewRange;
        Position = new float[3];
        Rotation = new float[3];
        if (go != null)
        {
            Position[0] = go.transform.position.x;
            Position[1] = go.transform.position.y;
            Position[2] = go.transform.position.z;
            Rotation[0] = go.transform.rotation.x;
            Rotation[1] = go.transform.rotation.y;
            Rotation[2] = go.transform.rotation.z;
        }
        player = new Player(p);
        FireRate = firerate;
        ID = 0;
        HitPoint = hitpoint;
        MaxHitPoint = HitPoint;
        AntiType = new int[4];
        initialAntiType = new int[4];
        antitype.CopyTo(AntiType, 0);
        antitype.CopyTo(initialAntiType, 0);
        TrackUnit = trackunit;
        TurningSpeed = turningspeed;
        UnitType = unittype;
        troops = new string[troop];
        currentTroop = 0;
        this.height = height;
        Ability = new List<AbilityData>();
        this.pellet = pellet;
        this.burst = burst;
        this.sta = sta;
        this.accuracy = accuracy;
        this.cost = cost;
        this.produce_time = produce_time;
        this.size = size;
        this.shield = shield;
        this.max_shield = max_shield;
    }

    public Unit(Unit unit)
    {
        Armor = unit.getArmor();
        Damage = unit.getDamage();
        Troop = unit.getTroop();
        TroopType = unit.getTroopType();
        TroopAttack = unit.isTroopAttackable();
        TroopOut = unit.isTroopOut();
        Type = unit.getType();
        Group = unit.getGroup();
        Speed = unit.getSpeed();
        ArmorPenetration = unit.getArmorPenetration();
        List<Equipment> equipments = unit.getEquipment();
        equipment = new List<Equipment>();
        equipment.Capacity = equipments.Capacity;
        for(int i = 0; i < equipments.Capacity; i++)
        {
            equipment.Add(new Equipment(equipments[i]));
        }
        Range = unit.getRange();
        normal_range = unit.getRange();
        ViewRange = unit.getViewRange();
        Position = new float[3];
        Rotation = new float[3];
        //unit.getPosition().CopyTo(Position, 0);
        //unit.getRotation().CopyTo(Rotation, 0);
        player = new Player(unit.getPlayer());
        FireRate = unit.getFireRate();
        ID = unit.getID();
        HitPoint = unit.getHitPoint();
        MaxHitPoint = unit.getMaxHitPoint();
        AntiType = new int[4];
        initialAntiType = new int[4];
        unit.getAntiType().CopyTo(AntiType, 0);
        unit.getAntiType().CopyTo(initialAntiType, 0);
        TrackUnit = unit.isTrackUnit();
        TurningSpeed = unit.getTurningSpeed();
        UnitType = unit.getUnitType();
        troops = new string[unit.getTroop()];
        currentTroop = 0;
        height = unit.getHeight();
        Ability = new List<AbilityData>();
        burst = unit.getBurst();
        sta = unit.isStatic();
        accuracy = unit.getAccuracy();
        pellet = unit.getPellet();
        cost = unit.getCost();
        produce_time = unit.getProduceTime();
        size = unit.getSize();
        shield = unit.getShield();
        max_shield = unit.getMaxShield();
    }

    public Unit(int hitpoint, int shield, int max_shield, int size, int armor, int damage, bool trackunit, string type, int group, float speed, float turningspeed, int armorpenetration, float range, float viewRange, float firerate, float accuracy, int pellet, int burst, int unittype, int[] antitype, float height, GameObject go, int equipnum, bool sta, int cost, float produce_time, Player p)
    {
        Armor = armor;
        Damage = damage;
        Troop = 0;
        TroopType = -1;
        TroopAttack = false;
        TroopOut = false;
        Type = type;
        Group = group;
        Speed = speed;
        ArmorPenetration = armorpenetration;
        Range = range;
        normal_range = range;
        ViewRange = viewRange;
        Position = new float[3];
        Rotation = new float[3];
        if (go != null)
        {
            Position[0] = go.transform.position.x;
            Position[1] = go.transform.position.y;
            Position[2] = go.transform.position.z;
            Rotation[0] = go.transform.rotation.x;
            Rotation[1] = go.transform.rotation.y;
            Rotation[2] = go.transform.rotation.z;
        }
        player = new Player(p);
        FireRate = firerate;
        ID = 0;
        HitPoint = hitpoint;
        MaxHitPoint = HitPoint;
        equipment = new List<Equipment>();
        equipment.Capacity = equipnum;
        initializeEquipment();
        AntiType = new int[4];
        initialAntiType = new int[4];
        antitype.CopyTo(AntiType, 0);
        antitype.CopyTo(initialAntiType, 0);
        TrackUnit = trackunit;
        TurningSpeed = turningspeed;
        UnitType = unittype;
        troops = new string[Troop];
        currentTroop = 0;
        this.height = height;
        Ability = new List<AbilityData>();
        this.pellet = pellet;
        this.burst = burst;
        this.sta = sta;
        this.accuracy = accuracy;
        this.cost = cost;
        this.produce_time = produce_time;
        this.size = size;
        this.shield = shield;
        this.max_shield = max_shield;
    }

    public Unit(int hitpoint, int shield, int max_shield, int size, int armor, int damage, int troop, int trooptype, bool troopattack, bool troopout, bool trackunit, string type, int group, float speed, float turningspeed, int armorpenetration, float range, float viewRange, float firerate, float accuracy, int pellet, int burst, int unittype, int[] antitype, float height, GameObject go, int equipnum, bool sta, int cost, float produce_time, Player p)
    {
        Armor = armor;
        Damage = damage;
        Troop = troop;
        TroopType = trooptype;
        TroopAttack = troopattack;
        TroopOut = troopout;
        Type = type;
        Group = group;
        Speed = speed;
        ArmorPenetration = armorpenetration;
        Range = range;
        normal_range = range;
        ViewRange = viewRange;
        Position = new float[3];
        Rotation = new float[3];
        if (go != null)
        {
            Position[0] = go.transform.position.x;
            Position[1] = go.transform.position.y;
            Position[2] = go.transform.position.z;
            Rotation[0] = go.transform.rotation.x;
            Rotation[1] = go.transform.rotation.y;
            Rotation[2] = go.transform.rotation.z;
        }
        player = new Player(p);
        FireRate = firerate;
        ID = 0;
        HitPoint = hitpoint;
        MaxHitPoint = HitPoint;
        equipment = new List<Equipment>();
        equipment.Capacity = equipnum;
        initializeEquipment();
        AntiType = new int[4];
        initialAntiType = new int[4];
        antitype.CopyTo(AntiType, 0);
        antitype.CopyTo(initialAntiType, 0);
        TrackUnit = trackunit;
        TurningSpeed = turningspeed;
        UnitType = unittype;
        troops = new string[troop];
        currentTroop = 0;
        this.height = height;
        Ability = new List<AbilityData>();
        this.pellet = pellet;
        this.burst = burst;
        this.sta = sta;
        this.accuracy = accuracy;
        this.cost = cost;
        this.produce_time = produce_time;
        this.size = size;
        this.shield = shield;
        this.max_shield = max_shield;
    }

    public Unit(string type, int hitpoint, int size, int armor, float speed, float range, float viewRange, int equipnum, int unitType, float height, int cost, float produce_time, GameObject go)
    {
        Armor = armor;
        Damage = 0;
        Troop = 0;
        TroopType = -1;
        TroopAttack = false;
        TroopOut = false;
        TrackUnit = false;
        Type = type;
        Group = -1;
        Speed = speed;
        ArmorPenetration = 0;
        ID = 0;
        HitPoint = hitpoint;
        MaxHitPoint = HitPoint;
        equipment = new List<Equipment>();
        equipment.Capacity = equipnum;
        initializeEquipment();
        Range = range;
        normal_range = range;
        ViewRange = viewRange;
        UnitType = unitType;
        AntiType = new int[] { 0, 0, 0, 0 };
        initialAntiType = new int[] { 0, 0, 0, 0 };
        Position = new float[3];
        Rotation = new float[3];
        if (go != null)
        {
            Position[0] = go.transform.position.x;
            Position[1] = go.transform.position.y;
            Position[2] = go.transform.position.z;
            Rotation[0] = go.transform.rotation.x;
            Rotation[1] = go.transform.rotation.y;
            Rotation[2] = go.transform.rotation.z;
        }
        TurningSpeed = 200;
        troops = new string[0];
        currentTroop = 0;
        this.height = height;
        burst = 0;
        Ability = new List<AbilityData>();
        sta = false;
        accuracy = 0;
        pellet = 0;
        player = null;
        this.cost = cost;
        this.produce_time = produce_time;
        this.size = size;
        shield = 0;
        max_shield = -1;
    }

    public bool addToTroop(Unit unit)
    {
        if (currentTroop >= Troop)
            return false;
        troops[currentTroop] = unit.getType();
        if (Range == 0 && unit.getDamage() != 0)
            Range = 30;
        currentTroop++;
        return true;
    }

    public bool isStatic()
    {
        return sta;
    }

    public float getAccuracy()
    {
        return accuracy;
    }

    public void setStatic(bool sta)
    {
        this.sta = sta;
    }

    public bool isTroopAttackable()
    {
        return TroopAttack;
    }

    public bool isTroopOut()
    {
        return TroopOut;
    }

    public int getTroopType()
    {
        return TroopType;
    }

    public int getBurst()
    {
        return burst;
    }

    public List<AbilityData> getAbility()
    {
        return Ability;
    }

    public string removeTroop(int index)
    {
        if (index > currentTroop || index < 0)
        {
            Debug.Log(index);
            return null;
        }
        string unit = troops[index];
        for (int i = index; i <= currentTroop; i++)
            troops[i] = troops[i + 1];
        currentTroop--;
        return unit;
    }

    public void clearTroop()
    {
        currentTroop = 0;
        troops = new string[Troop];
    }

    public string[] getCurrentTroops()
    {
        return troops;
    }

    private void initializeEquipment()
    {
        for (int i = 0; i < equipment.Capacity; i++)
            equipment.Add(new Equipment());
    }

    public float getTurningSpeed()
    {
        return TurningSpeed;
    }

    public float getHeight()
    {
        return height;
    }

    public int getUnitType()
    {
        return UnitType;
    }

    public float getFireRate()
    {
        return FireRate;
    }

    public Player getPlayer()
    {
        return player;
    }

    public float getViewRange()
    {
        return ViewRange;
    }

    public float getRange()
    {
        return Range;
    }

    public int getArmorPenetration()
    {
        return ArmorPenetration;
    }

    public int getTroop()
    {
        return Troop;
    }

    public int getArmor()
    {
        return Armor;
    }

    public int getDamage()
    {
        return Damage;
    }

    public string getType()
    {
        return Type;
    }

    public int getGroup()
    {
        return Group;
    }

    public float getSpeed()
    {
        return Speed;
    }

    public bool isFlyUnit()
    {
        return UnitType / 10 == 2;
    }

    public bool isInfantry()
    {
        return UnitType / 10 == 0;
    }

    public bool isBuilding()
    {
        return UnitType / 10 == 3;
    }

    public float[] getPosition()
    {
        return Position;
    }

    public float[] getRotation()
    {
        return Rotation;
    }

    public int getMaxHitPoint()
    {
        return MaxHitPoint;
    }

    public void setEquipment(Equipment equip)
    {
        if (!equip.getUnit().Equals(Type))
        {
            Debug.Log("Equipment and type incompatible!");
            return;
        }
        if (!equipment[equip.getIndex()].getType().Equals("Blank"))
            resetEquipment(equip.getIndex());
            
        Damage += equip.getDamage();
        Speed += equip.getSpeed();
        ArmorPenetration += equip.getArmorPenetration();
        Armor += equip.getArmor();
        if(normal_range == Range)
            Range += equip.getRange();
        normal_range += equip.getRange();
        ViewRange += equip.getViewRange();
        FireRate += equip.getFireRate();
        AntiType = new int[4];
        equip.getAntiType().CopyTo(AntiType, 0);
        equipment[equip.getIndex()] = new Equipment(equip);
        AbilityData[] a = equip.getAbility();
        if (a.Length > 0)
            for (int i = 0; i < a.Length; i++)
                Ability.Add(a[i]);
        burst = equip.burstFire();
        pellet = equip.getPellet();
    }

    public void setEquipment(List<Equipment> equip)
    {
        if(equip.Count > equipment.Capacity - equipment.Count)
        {
            Debug.Log("Too many equipment in the given equipment list");
            return;
        }
        foreach (Equipment e in equip)
        {
            setEquipment(e);
        }
    }

    public void replaceEquipment(List<Equipment> equip)
    {
        if (equip.Count > equipment.Capacity)
        {
            Debug.Log("Too many equipment in the given replaced equipment list");
            return;
        }
        int equipnum = equipment.Capacity;
        resetEquipment();
        foreach(Equipment e in equip)
        {
            setEquipment(e);
        }
    }

    public void resetEquipment()
    {
        foreach (Equipment e in equipment)
        {
            Damage -= e.getDamage();
            Speed -= e.getSpeed();
            if(Range == normal_range)
                Range -= e.getRange();
            normal_range -= e.getRange();
            ViewRange -= e.getViewRange();
            FireRate -= e.getFireRate();
            Armor -= e.getArmor();
            ArmorPenetration -= e.getArmorPenetration();
            burst -= e.burstFire();
            pellet -= e.getPellet();
        }
        int capacity = equipment.Capacity;
        equipment = new List<Equipment>(capacity);
        for (int i = 0; i < capacity; i++)
            equipment.Add(new Blank());
        initialAntiType.CopyTo(AntiType, 0);
        Ability = new List<AbilityData>();
    }

    void resetEquipment(int index)
    {
        Equipment e = equipment[index];
        Damage -= e.getDamage();
        Speed -= e.getSpeed();
        if(normal_range == Range)
            Range -= e.getRange();
        normal_range -= e.getRange();
        ViewRange -= e.getViewRange();
        FireRate -= e.getFireRate();
        Armor -= e.getArmor();
        ArmorPenetration -= e.getArmorPenetration();
        if (!Enumerable.SequenceEqual(e.getAntiType(), initialAntiType))
            initialAntiType.CopyTo(AntiType, 0);
        AbilityData[] a = equipment[index].getAbility();
        for(int i = Ability.Count - 1; i >= 0 && Ability.Count != 0; i--)
            for(int j = 0; j < a.Length; j++)
                if (Ability[i].getAbilityType().Equals(a[j].getAbilityType()))
                {
                    Ability.RemoveAt(i);
                    break;
                }
        equipment[index] = new Blank();
        
    }

    public void generateID(List<Unit> units)
    {
        if (ID != 0)
            return;
        ID = Convert.ToInt64(Random.Range(1f, 9999999999f));
        foreach (Unit unit in units)
        {
            if (unit.getID() == ID)
            {
                ID = 0;
                generateID(units);
            }
        }
    }

    public bool setRange(float range)
    {
        if (range > ViewRange)
        {
            Debug.Log("range is larger than view range!");
            return false;
        }
        if (range > Range && Range != 0)
            return false;
        Range = range;
        return true;
    }

    public void resetRange()
    {
        Range = normal_range;
    }

    public long getID()
    {
        return ID;
    }

    public int getPellet()
    {
        return pellet;
    }

    public void setHitPoint(int hitpoint, int armorpenetration, int[] antiType)
    {
        int typeIndex = UnitType / 10;
        int armorpoint = Mathf.Max(0, Armor - armorpenetration);
        int uType = UnitType % 10;
        int state = hitpoint % 10;
        hitpoint /= 10;
        switch (state)
        {
            case 1:
                HitPoint += hitpoint;
                break;
            case 2:
                if (antiType[typeIndex] == uType)
                    hitpoint *= 2;
                else if(antiType[typeIndex] != uType && antiType[typeIndex] != 3)
                    hitpoint /= 2;
                if (antiType[typeIndex] < uType && antiType[typeIndex] != 0)
                {
                    float rate = 0;
                    rate = Random.Range(0f, 100f);
                    rate *= (float)HitPoint / MaxHitPoint;
                    if(rate <= 50)
                        HitPoint -= Mathf.Max(hitpoint - armorpoint, 1);
                }
                else if(antiType[typeIndex] > uType && antiType[typeIndex] != 3)
                {
                    hitpoint /= 5;
                    HitPoint -= Mathf.Max(hitpoint - armorpoint, 1);
                }
                else if(antiType[typeIndex] == 0)
                {
                    hitpoint /= 2;
                    HitPoint -= Mathf.Max(hitpoint - armorpoint, 1);
                }
                else
                    HitPoint -= Mathf.Max(hitpoint - armorpoint, 1);
                break;
            case 3:
                HitPoint = hitpoint;
                break;
            case 0:
                HitPoint = 0;
                break;
            default:
                Debug.Log("HitPoint input invalid!");
                break;
        }
        HitPoint = (int)Mathf.Clamp(HitPoint, 0f, MaxHitPoint);
    }

    public bool isTrackUnit()
    {
        return TrackUnit;
    }

    public int getHitPoint()
    {
        return HitPoint;
    }

    public List<Equipment> getEquipment()
    {
        return equipment;
    }

    public int[] getAntiType()
    {
        return AntiType;
    }

    public int getCost()
    {
        return cost;
    }

    public float getProduceTime()
    {
        return produce_time;
    }

    public int getSize()
    {
        return size;
    }

    public int getShield()
    {
        return shield;
    }

    public int getMaxShield()
    {
        return max_shield;
    }

    public void setPositionAndRotation(GameObject gameobj)
    {
        Position[0] = gameobj.transform.position.x;
        Position[1] = gameobj.transform.position.y;
        Position[2] = gameobj.transform.position.z;
        Rotation[0] = gameobj.transform.rotation.x;
        Rotation[1] = gameobj.transform.rotation.y;
        Rotation[2] = gameobj.transform.rotation.z;
    }

    public void setPlayer(Player player)
    {
        if (this.player != null)
            return;
        this.player = player;
    }

    public void setGroup(int group)
    {
        Group = group;
    }

    public float getNormalRange()
    {
        return normal_range;
    }
}
