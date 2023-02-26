using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLoad : MonoBehaviour
{
    Unit unit;
    [SerializeField] GameObject dmarker;
    Vector3 minimap;
    float ratio;
    GameObject marker;
    public int group;
    public HealthBar healthbar;
    GameObject range;
    GameObject viewRange;
    List<Player> players;
    List<GameObject> equipments;
    Attack attack;
    [SerializeField] Transform Equipment;
    [SerializeField] GameObject Smoke;
    [SerializeField] GameObject Out;
    Effect[] effects;
    float[] effectTime;
    GameObject[] troops;
    HashSet<string> TroopName;
    int currentTroop;
    bool initiated = false;
    bool aActive;
    float aDistance;
    float tDistance;
    MovementControl control;
    AbilityAbs ability;
    int Destroying;
    float speed;
    int indx_tmp;
    AbilityLoad aload;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        aload = null;
        transform.parent.TryGetComponent<AbilityLoad>(out aload);
        Destroying = -1;
        aActive = false;
        ability = null;
        ratio = 300 / 1920.0f * Screen.width / 2;
        minimap = manager.getMiniMap().transform.position;
        if(dmarker != null)
            marker = Instantiate(dmarker, manager.getMiniMap().transform);
        aDistance = 0;
        tDistance = 0;
        range = transform.parent.GetChild(3).gameObject;
        viewRange = transform.parent.GetChild(4).gameObject;
        players = manager.getPlayers();
        Player player = new Player(players[0]);
        string name = transform.name;
        unit = new Unit(manager.getNewUnit(name));
        UnitLoad u;
        if(transform.parent.parent != null && transform.parent.parent.TryGetComponent<UnitLoad>(out u))
        {
            Unit pUnit = u.OutputUnit();
            if (pUnit == null)
                return;
            Unit thisU = new Unit(unit);
            unit = null;
            unit = new Unit(1, pUnit.getShield(), pUnit.getMaxShield(), pUnit.getSize(), pUnit.getArmor(), thisU.getDamage(), thisU.isTrackUnit(), thisU.getType(), pUnit.getGroup(), thisU.getSpeed(),
                            thisU.getTurningSpeed(), thisU.getArmorPenetration(), thisU.getRange(), thisU.getViewRange(), thisU.getFireRate(), thisU.getAccuracy(), thisU.getPellet(), thisU.getBurst(),
                            pUnit.getUnitType(), thisU.getAntiType(), thisU.getHeight(), transform.parent.gameObject, thisU.getEquipment().Capacity, thisU.isStatic(), thisU.getCost(), thisU.getProduceTime(), pUnit.getPlayer());
            group = pUnit.getGroup();
        }
        else
        {
            unit.setPositionAndRotation(transform.parent.gameObject);
            unit.setPlayer(player);
            unit.setGroup(group);
        }
        troops = new GameObject[unit.getTroop()];
        TroopName = new HashSet<string>(unit.getTroop());
        currentTroop = 0;
        effects = new Effect[Data.EffectNumber];
        for (int i = 0; i < Data.EffectNumber; i++)
            effects[i] = null;
        effectTime = new float[Data.EffectNumber];
        equipments = new List<GameObject>();
        attack = transform.parent.GetChild(3).GetComponent<Attack>();
        equipments.Capacity = unit.getEquipment().Capacity;
        for (int i = 0; i < equipments.Capacity; i++)
            equipments.Add(null);
        for(int i = 0; i < equipments.Capacity; i++)
        {
            if (Equipment.GetChild(i).childCount == 0)
                continue;
            EquipmentLoad eload = Equipment.GetChild(i).GetChild(0).GetComponent<EquipmentLoad>();
            equipments[eload.getEquipment().getIndex()] = Equipment.GetChild(i).GetChild(0).gameObject;
            unit.setEquipment(Equipment.GetChild(i).GetChild(0).GetComponent<EquipmentLoad>().getEquipment());
            attack.setBarrel(eload.getBarrelBody(), eload.getBarrel(), eload.getBullet(), eload.getBulletOut());
            foreach (AbilityData abd in eload.getEquipment().getAbility())
            {
                System.Type type = System.Type.GetType(abd.getAbilityType() + ",Assembly-CSharp");
                if (transform.parent.GetComponent(abd.getAbilityType()) == null)
                    transform.parent.gameObject.AddComponent(type);
            }
        }
        if(healthbar != null)    
            healthbar.SetMaxHealth(unit.getMaxHitPoint());
        SphereCollider s_collider;
        CapsuleCollider c_collider;
        if (range.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = unit.getRange();
        else if (range.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = unit.getRange();
        if (viewRange.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = unit.getViewRange();
        else if (viewRange.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = unit.getViewRange();
        InRange r;
        if (transform.parent.GetChild(5).TryGetComponent<InRange>(out r))
            r.setRadius(unit.getRange());

        GenerateID(manager.getUnitList());
        if(transform.parent.parent == null)
            manager.addNewUnit(unit);
        initiated = true;
    }

    private void Start()
    {
        transform.parent.TryGetComponent<MovementControl>(out control);
        if (initiated)
            return;
        range = transform.parent.GetChild(3).gameObject;
        viewRange = transform.parent.GetChild(4).gameObject;
        players = GameObject.Find("EventSystem").GetComponent<GameManager>().getPlayers();
        Player player = new Player(players[0]);
        string name = transform.name;
        
        UnitLoad u;
        if (transform.parent.parent != null && transform.parent.parent.TryGetComponent<UnitLoad>(out u))
        {
            Unit pUnit = u.OutputUnit();
            if (pUnit == null)
                Debug.Log("notLoad");
            Unit thisU = new Unit(unit);
            unit = null;
            unit = new Unit(1, pUnit.getShield(), pUnit.getMaxShield(), pUnit.getSize(), pUnit.getArmor(), thisU.getDamage(), thisU.isTrackUnit(), thisU.getType(), pUnit.getGroup(), thisU.getSpeed(),
                            thisU.getTurningSpeed(), thisU.getArmorPenetration(), thisU.getRange(), thisU.getViewRange(), thisU.getFireRate(), thisU.getAccuracy(), thisU.getPellet(), thisU.getBurst(), pUnit.getUnitType(),
                            thisU.getAntiType(), thisU.getHeight(), transform.parent.gameObject, thisU.getEquipment().Capacity, thisU.isStatic(), thisU.getCost(), thisU.getProduceTime(), pUnit.getPlayer());
            group = pUnit.getGroup();
        }
        else
        {
            unit = new Unit(GameObject.Find("EventSystem").GetComponent<GameManager>().getNewUnit(name));
            unit.setPositionAndRotation(transform.parent.gameObject);
            unit.setPlayer(player);
            unit.setGroup(group);
        }
        troops = new GameObject[unit.getTroop()];
        currentTroop = 0;
        effects = new Effect[Data.EffectNumber];
        for (int i = 0; i < Data.EffectNumber; i++)
            effects[i] = null;
        effectTime = new float[Data.EffectNumber];
        equipments = new List<GameObject>();
        attack = transform.parent.GetChild(3).GetComponent<Attack>();
        equipments.Capacity = unit.getEquipment().Capacity;
        for (int i = 0; i < equipments.Capacity; i++)
            equipments.Add(null);
        GameManager manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        for (int i = 0; i < equipments.Capacity; i++)
        {
            EquipmentLoad eload = Equipment.GetChild(i).GetChild(0).GetComponent<EquipmentLoad>();
            equipments[eload.getEquipment().getIndex()] = Equipment.GetChild(i).GetChild(0).gameObject;
            unit.setEquipment(Equipment.GetChild(i).GetChild(0).GetComponent<EquipmentLoad>().getEquipment());
            attack.setBarrel(eload.getBarrelBody(), eload.getBarrel(), eload.getBullet(), eload.getBulletOut());
            foreach (AbilityData abd in eload.getEquipment().getAbility())
            {
                System.Type type = System.Type.GetType(abd.getAbilityType() + ",Assembly-CSharp");
                if (transform.parent.GetComponent(abd.getAbilityType()) == null)
                    transform.parent.gameObject.AddComponent(type);
            }
        }
        if (healthbar != null)
            healthbar.SetMaxHealth(unit.getMaxHitPoint());
        SphereCollider s_collider;
        CapsuleCollider c_collider;
        if (range.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = unit.getRange();
        else if (range.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = unit.getRange();
        InRange r;
        if (transform.parent.GetChild(5).TryGetComponent<InRange>(out r))
            r.setRadius(unit.getRange());

        GenerateID(manager.getUnitList());
        if(transform.parent.parent == null)
            manager.addNewUnit(unit);
        initiated = true;
    }

    public void setAbilityActive(bool set)
    {
        aActive = set;
    }    

    public bool isInitiated()
    {
        return initiated;
    }

    void Update()
    {
        calcDistance();
        if(marker != null)
            marker.transform.position = minimap +  new Vector3(transform.parent.position.x / 500 * ratio, transform.parent.position.z / 500 * ratio, 0);
        if (aActive && aDistance < tDistance)
        {
            if(control != null)
                control.cancelTarget();
            ability.execute();
            aload.Use(indx_tmp);
            aActive = false;
        }
        else if (aActive && control != null && control.isIdle())
            control.MoveTo(transform.parent.GetComponent<AbilityAbs>().getTargetVector(), false);
        if(healthbar != null)
            healthbar.SetCurrentHealth(unit.getHitPoint());
        if(Smoke != null)
        {
            if (unit.getHitPoint() / (float)unit.getMaxHitPoint() < 0.4)
                Smoke.SetActive(true);
            else
                Smoke.SetActive(false);
        }
        setPositionAndRotation(transform.parent.gameObject);
        if (unit.getHitPoint() <= 0)
        {
            //transform.parent.GetComponent<InstructionQueue>().Destroy();
            //Destroy(transform.parent.gameObject);
            GetComponent<Animator>().SetTrigger("Destroy");
        }
        for(int i = 0; i < Data.EffectNumber; i++)
        {
            if (effects[i] == null)
                continue;
            if (Time.time - effectTime[i] > effects[i].getTime())
                effects[i] = null;
        }
        for (int i = 0; i < currentTroop; i++)
            troops[i].transform.position = transform.parent.position;
    }

    private void FixedUpdate()
    {
        if(Destroying > -1)
        {
            if(transform.name.Equals("Assaulter"))
            {
                if (transform.position.y <= 2 && !transform.Find("Destroy").GetChild(4).gameObject.activeSelf)
                    transform.Find("Destroy").GetChild(4).gameObject.SetActive(true);
                if (transform.position.y > 2)
                {
                    if (transform.eulerAngles.x > 180 || transform.eulerAngles.x < 45)
                        transform.eulerAngles += new Vector3(100 * Time.deltaTime, 0, 0);
                }
                else
                    transform.eulerAngles -= new Vector3(transform.eulerAngles.x, 0, 0);
                transform.position -= new Vector3(0, transform.position.y > 2 ? 20 * Time.deltaTime : 0, 0);
            }
            if (transform.position.y <= 10)
                speed = Mathf.Max(0, speed - 20 * Time.deltaTime);
            if (unit.isFlyUnit() && Destroying == 1)
            {
                transform.parent.position += transform.forward * Time.deltaTime * speed;
                transform.parent.position += new Vector3(0, 20 - transform.parent.position.y, 0);
            }
        }
    }

    public void calcDistance()
    {
        AbilityAbs ability;
        if(aActive && transform.parent.TryGetComponent<AbilityAbs>(out ability))
        {
            Vector3 current = transform.parent.position;
            Vector3 target = ability.getTargetVector();
            aDistance = (current.x - target.x) * (current.x - target.x) + (current.z - target.z) * (current.z - target.z);
        }
    }

    public Unit OutputUnit()
    {
        return unit;
    }

    public void ActiveAbility(int index, Vector3 pos)
    {
        if(index >= unit.getAbility().Count)
        {
            Debug.Log(index + " is larger than the list count: " + unit.getAbility().Count);
            return;
        }
        AbilityData aData = unit.getAbility()[index];
        ability = transform.parent.GetComponent(aData.getAbilityType()) as AbilityAbs;

        if(ability != null)
        {
            Attack attack = transform.parent.GetChild(3).GetComponent<Attack>();
            
            if(!aData.getObjectName().Equals(""))
            {
                GameObject obj = Resources.Load(aData.getObjectName()) as GameObject;
                ability.set(aData.getGroup(), attack.getBulletOut(), obj, null, aData.getRadius(), aData.getDistance(), aData.getDuration(), aData.getDamage(), aData.getArmorPenetration(), aData.getAntiType(), aData.getEffect());
            }
            else
                ability.set(aData.getGroup(), attack.getBulletOut(), attack.getBullet(), null, aData.getRadius(), aData.getDistance(), aData.getDuration(), aData.getDamage(), aData.getArmorPenetration(), aData.getAntiType(), aData.getEffect());
            ability.setTargetVector(pos);
            aActive = true;
            tDistance = aData.getDistance() * aData.getDistance();
            indx_tmp = index;
        }
        else
        {
            Debug.Log(transform.parent.name + " does not have ability component " + aData.getAbilityType());
            return;
        }
    }

    public void GenerateID(List<Unit> units)
    {
        if (unit == null)
            return;
        unit.generateID(units);
        transform.parent.name = "Unit" + unit.getID();
    }
    
    public void setPositionAndRotation(GameObject gameobj)
    {
        unit.setPositionAndRotation(gameobj);
    }

    public void addEquipment(Equipment equipment)
    {
        if (!equipment.getUnit().Equals(unit.getType()))
            return;
        //if (Equipment.childCount > 0)
           // Destroy(Equipment.GetChild(0).gameObject);
        if (Equipment.GetChild(equipment.getIndex()).childCount > 0)
        {
            foreach(AbilityData abd in equipments[equipment.getIndex()].GetComponent<EquipmentLoad>().getEquipment().getAbility())
            {
                Destroy(transform.parent.GetComponent(abd.getAbilityType()));
            }
            Destroy(Equipment.GetChild(equipment.getIndex()).GetChild(0).gameObject);
        }
        GameObject gameobj = Resources.Load(equipment.getName()) as GameObject;
        GameObject e = Instantiate(gameobj, Equipment.GetChild(equipment.getIndex()));
        Transform current = Equipment.transform;
        while(current.parent != null)
        {
            current = current.parent;
            e.transform.localScale = new Vector3(e.transform.localScale.x / current.localScale.x, e.transform.localScale.y / current.localScale.y, e.transform.localScale.z / current.localScale.z);
        }
        EquipmentLoad eload = e.GetComponent<EquipmentLoad>();
        equipments[equipment.getIndex()] = e;
        //for (int i = 2; i < e.transform.childCount; i++)
          //  bulletOut[i - 2] = e.transform.GetChild(i).gameObject;
        attack.setBarrel(eload.getBarrelBody(), eload.getBarrel(), eload.getBullet(), eload.getBulletOut());
        unit.setEquipment(equipment);
        foreach(AbilityData abd in equipment.getAbility())
        {
            System.Type type = System.Type.GetType(abd.getAbilityType() + ",Assembly-CSharp");
            if (transform.parent.GetComponent(type) == null)
                transform.parent.gameObject.AddComponent(type);
        }
        SphereCollider s_collider;
        CapsuleCollider c_collider;
        if (range.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = unit.getRange();
        else if (range.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = unit.getRange();
        attack.setRadius(unit.getRange());
        transform.parent.GetChild(5).GetComponent<InRange>().setRadius(unit.getRange());
        //attack.setBarrel(eload.getBarrelBody(), eload.getBarrel(), eload.getBullet(), eload.getBulletOut());
    }

    public void resetEquipment()
    {
        equipments = new List<GameObject>(unit.getEquipment().Capacity);
        for (int i = 0; i < equipments.Capacity; i++)
            equipments.Add(null);
        //if (Equipment.childCount > 0)
          //  Destroy(Equipment.GetChild(0).gameObject);
        foreach(Transform tr in Equipment)
        {
            if (tr.childCount > 0)
                Destroy(tr.GetChild(0).gameObject);
        }
        attack.resetBarrel();
        unit.resetEquipment();
        SphereCollider s_collider;
        CapsuleCollider c_collider;
        if (range.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = unit.getRange();
        else if (range.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = unit.getRange();
    }

    public void setHitPoint(int hitpoint, int armorpenetration, int[] antitype)
    {
        UnitLoad u;
        if (transform.parent.parent != null && transform.parent.parent.TryGetComponent<UnitLoad>(out u))
            u.setHitPoint(hitpoint, armorpenetration, antitype);
        else
            unit.setHitPoint(hitpoint, armorpenetration, antitype);
    }

    public bool addTroop(GameObject gameobj)
    {
        if (!unit.addToTroop(gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit()))
            return false;
        gameobj.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        gameobj.transform.GetChild(1).gameObject.SetActive(false);
        gameobj.transform.GetChild(0).GetComponent<UnitLoad>().setSmokeActive(false);
        gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().setStatic(false);
        setRange(gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getRange());
        GroundUnitCollision col;
        FlyUnitCollision fCol;
        if (gameobj.TryGetComponent<GroundUnitCollision>(out col))
            col.setActive(false);
        else if (gameobj.transform.GetChild(2).TryGetComponent<FlyUnitCollision>(out fCol))
            fCol.setActive(false);
        Attack a;
        if (gameobj.transform.GetChild(3).TryGetComponent<Attack>(out a))
        {
            if(!unit.isTroopAttackable())
                a.setActive(false);
            a.set_in_vehicle(true);
        }
        InstructionQueue queue;
        if (gameobj.TryGetComponent<InstructionQueue>(out queue))
            queue.clearQueue();
        troops[currentTroop] = gameobj;
        TroopName.Add(gameobj.name);
        currentTroop++;
        return true;
    }

    private void setRange(float Range)
    {
        if (!unit.setRange(Range))
            return;
        SphereCollider s_collider;
        CapsuleCollider c_collider;
        if (range.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = Range;
        else if (range.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = Range;
        InRange r;
        if (transform.parent.GetChild(5).TryGetComponent<InRange>(out r))
            r.setRadius(Range);
    }

    private void resetRange()
    {
        unit.resetRange();
        SphereCollider s_collider;
        CapsuleCollider c_collider;
        if (range.TryGetComponent<SphereCollider>(out s_collider))
            s_collider.radius = unit.getRange();
        else if (range.TryGetComponent<CapsuleCollider>(out c_collider))
            c_collider.radius = unit.getRange();
        InRange r;
        if (transform.parent.GetChild(5).TryGetComponent<InRange>(out r))
            r.setRadius(unit.getRange());
    }

    public bool troopInVehicle(GameObject target)
    {
        /*for(int i = 0; i < currentTroop; i++)
            if (troops[i] == target)
                return true;*/
        return TroopName.Contains(target.name);
    }

    public GameObject[] getTroops()
    {
        return troops;
    }

    public int getCurrentTroops()
    {
        return currentTroop;
    }

    public bool removeTroop(int index)
    {
        if (unit.removeTroop(index) == null)
            return false;
        if (index >= currentTroop)
            return false;
        GameObject gameobj = troops[index];
        TroopName.Remove(gameobj.name);
        gameobj.GetComponentInChildren<MeshRenderer>().enabled = true;
        gameobj.transform.GetChild(1).gameObject.SetActive(true);
        gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().setStatic(false);
        float min_range = unit.getNormalRange();
        Unit ct;
        foreach(GameObject troop in troops)
        {
            ct = troop.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
            if (ct.getRange() < min_range)
                min_range = ct.getRange();
        }
        setRange(min_range);
        GroundUnitCollision col;
        FlyUnitCollision fCol;
        if (gameobj.TryGetComponent<GroundUnitCollision>(out col))
            col.setActive(true);
        else if (gameobj.transform.GetChild(2).TryGetComponent<FlyUnitCollision>(out fCol))
            fCol.setActive(true);
        gameobj.transform.GetChild(3).GetComponent<Attack>().setActive(true);
        Transform Out = this.Out.transform;
        if (Out.childCount == 0)
            gameobj.transform.position = Out.position;
        else
            gameobj.transform.position = Out.GetChild(Random.Range(0, Out.childCount - 1)).position;
        gameobj.transform.position = new Vector3(gameobj.transform.position.x, gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHeight(), gameobj.transform.position.z);
        if(!unit.isTroopOut())
            gameobj.GetComponent<InstructionQueue>().addInstruction(new Instruction(4));
        gameobj.GetComponent<InstructionQueue>().addAndStash(new Instruction(0, Out.position + Out.forward * 10));
        for (int i = index; i <= currentTroop - 1; i++)
            troops[i] = troops[i + 1];
        currentTroop--;
        return true;
    }

    public void removeAllTroop()
    {
        GameObject gameobj;
        Transform Out = transform.parent.Find("Out");
        for (int i = 0; i < currentTroop; i++)
        {
            gameobj = troops[i];
            gameobj.GetComponentInChildren<MeshRenderer>().enabled = true;
            gameobj.transform.GetChild(1).gameObject.SetActive(true);
            GroundUnitCollision col;
            FlyUnitCollision fCol;
            if (gameObject.TryGetComponent<GroundUnitCollision>(out col))
                col.setActive(true);
            else if (gameObject.transform.GetChild(2).TryGetComponent<FlyUnitCollision>(out fCol))
                fCol.setActive(true);
            gameobj.transform.GetChild(3).GetComponent<Attack>().setActive(true);
            gameobj.transform.GetChild(3).GetComponent<Attack>().set_in_vehicle(true);
            if (Out.childCount == 0)
                gameobj.transform.position = Out.position;
            else
                gameobj.transform.position = Out.GetChild(Random.Range(0, Out.childCount - 1)).position;
            gameobj.transform.position = new Vector3(gameobj.transform.position.x, gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHeight(), gameobj.transform.position.z);
            if(!unit.isTroopOut())
                gameobj.GetComponent<InstructionQueue>().addInstruction(new Instruction(4));
            gameobj.GetComponent<InstructionQueue>().addAndStash(new Instruction(0, Out.position + Out.forward * 10));
        }
            
        troops = new GameObject[troops.Length];
        currentTroop = 0;
    }

    private void OnDestroy()
    {
        foreach (GameObject gameobj in troops)
            Destroy(gameobj);
    }

    public void StartDestroy()
    {
        InstructionQueue queue;
        if (transform.parent.TryGetComponent<InstructionQueue>(out queue))
            Destroy(queue);
        Destroy(transform.parent.GetComponent<ClickMe>());
        if (control != null)
        {
            if (!control.isIdle())
                Destroying = 1;
            else
                Destroying = 0;
            Destroy(control);
        }
        GroundUnitCollision collision;
        if (transform.parent.TryGetComponent<GroundUnitCollision>(out collision))
            Destroy(collision);
        GameObject[] toDestroy = new GameObject[5];
        for (int i = 1; i <= 5; i++)
            toDestroy[i - 1] = transform.parent.GetChild(i).gameObject;
        for (int i = 0; i < 5; i++)
            Destroy(toDestroy[i]);
        speed = unit.getSpeed();
        if (Destroying == -1)
            Destroying = 0;
        if (marker != null)
            Destroy(marker);
        if (GameObject.Find("EventSystem") == null)
            return;
        GameManager manager;
        if (GameObject.Find("EventSystem").TryGetComponent<GameManager>(out manager))
        {
            manager.removeUnit(unit);
            GameObject.Find("Main Camera").GetComponent<UnitSelection>().removeAt(transform.parent.gameObject);
            manager.SetUpSelectedBar();
        }
    }

    public void SetGroup(int group)
    {
        unit.setGroup(group);
        this.group = group;
    }

    public void setSmokeActive(bool active)
    {
        if(Smoke != null)
            Smoke.SetActive(active);
    }

    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
