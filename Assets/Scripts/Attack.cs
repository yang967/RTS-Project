using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Attack : MonoBehaviour
{
    UnitLoad load;
    protected Unit thisUnit;
    protected MovementControl control;
    GameObject thisUnitObject;
    protected GameObject target;
    GameObject sTarget;
    [SerializeField] protected GameObject bullet;
    [SerializeField] GameObject barrelbody;
    GameObject initialBarrelBody;
    [SerializeField] GameObject barrel;
    GameObject initialBarrel;
    [SerializeField] protected GameObject[] bulletOut;
    GameObject[] initialBulletOut;
    protected HashSet<GameObject> TargetList;
    protected HashSet<string> TargetUnits;
    string currentName;
    protected int index;
    protected int currentBurst;
    bool bursting;
    protected float time;
    protected float radius;
    float distance;
    protected bool attacking;
    protected Animator animator;
    bool disabled;
    float disabledTime;
    float currentTime;
    float angle;
    protected bool locked;
    bool sta;
    protected bool active = true;
    bool cancelled;
    InstructionQueue queue;
    bool in_vehicle;
    protected bool in_range;
    bool set_initial;
    Vector3 BarrelFixData, BarrelBodyFixData;
    float TargetCount;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        disabled = false;
        TargetList = new HashSet<GameObject>();
        TargetUnits = new HashSet<string>();
        currentName = "";
        initialBarrelBody = barrelbody;
        initialBarrel = barrel;
        thisUnitObject = transform.parent.GetChild(0).gameObject;
        load = thisUnitObject.GetComponent<UnitLoad>();
        thisUnit = load.OutputUnit();
        SphereCollider collider;
        if (gameObject.TryGetComponent<SphereCollider>(out collider))
            radius = collider.radius;
        else
            radius = gameObject.GetComponent<CapsuleCollider>().radius;
        time = 0;
        attacking = false;
        initialBulletOut = new GameObject[bulletOut.Length];
        bulletOut.CopyTo(initialBulletOut, 0);
        if (transform.parent.GetChild(0).GetComponent<Animator>() != null)
            animator = transform.parent.GetChild(0).GetComponent<Animator>();
        else
            animator = null;
        locked = false;
        sta = thisUnit.isStatic();
        queue = transform.parent.GetComponent<InstructionQueue>();
        in_vehicle = false;
        currentBurst = 0;
        index = 0;
        bursting = false;
        in_range = false;
        set_initial = false;
        control = transform.parent.GetComponent<MovementControl>();
        if(barrelbody != null)
            BarrelBodyFixData = barrelbody.transform.eulerAngles;
        if (barrel != null)
            BarrelFixData = barrel.transform.eulerAngles;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (!active)
            return;
        if (disabled)
        {
            if (Time.time - currentTime >= disabledTime)
                disabled = false;
            else
                return;
        }

        TargetCount = TargetList.Count;
        
        if (target != null && target.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() <= 0)
        {
            TargetList.Remove(target);
            TargetUnits.Remove(target.name);
            target = null;
        }
        /*if (TargetList.Count > 0 && (TargetList[0] == null || TargetList[0].transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() <= 0))
        {
            try
            {
                TargetUnits.Remove(TargetList[0].name);
                TargetList.RemoveAt(0);
            } catch(MissingReferenceException)
            {
                try
                {
                    GameManager manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
                    foreach (string str in TargetUnits)
                        if (!manager.IsUnitExists(str))
                            TargetUnits.Remove(str);
                }
                catch (System.InvalidOperationException) { }
            }
        }*/
        if (target == null && !in_vehicle && queue.isIdle() && TargetList.Count > 0 
            && (thisUnit.getDamage() > 0 || (load.getCurrentTroops() > 0 && thisUnit.isTroopAttackable())))
        {
            HashSet<GameObject>.Enumerator em = TargetList.GetEnumerator();
            /*while (TargetList.Count > 0 && target == null)
            {
                em.MoveNext();
                current = em.Current;
                if (current == null || thisUnit.getAntiType()[current.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getUnitType() / 10] == -1)
                {
                    TargetList.Remove(current);
                    continue;
                }
                target = current;
            }*/
            target = NextTarget();
            if(TargetList.Count > 0 && queue != null)
            {
                em.MoveNext();
                queue.addInstruction(new Instruction(1, em.Current));
            }
            else if(TargetList.Count > 0)
            {
                em.MoveNext();
                target = em.Current;
                currentName = target.name;
                //setTroopTarget(TargetList[0]);
                in_range = false;
                locked = false;
                calculate();
            }
        }
        if (target != null)
        {
            if (animator != null)
                animator.SetBool("Attack", true);
            Chase();
            if (sta || control.isIdle() || attacking)
            {
                if(!set_initial)
                {
                    if (transform.parent.GetChild(0).name.Equals("BattleBot"))
                        barrelbody.transform.eulerAngles += new Vector3(0, -90, 0);
                    set_initial = true;
                }
                if (!barrelSpinning())
                    return;
                attack();
            }
        }
        else
        {
            TargetList.Remove(target);
            TargetUnits.Remove(currentName);
            if (animator != null)
                animator.SetBool("Attack", false);
        }

        if (!locked)
            calculate();
        getDistance();

    }

    public void set_in_vehicle(bool in_vehicle)
    {
        this.in_vehicle = in_vehicle;
    }

    void calculate()
    {
        if (target == null || barrelbody == null)
            return;
        Quaternion rotation = Quaternion.LookRotation((target.transform.position - barrelbody.transform.position).normalized);
        rotation.eulerAngles += BarrelBodyFixData;
        angle = rotation.eulerAngles.y - barrelbody.transform.eulerAngles.y;
        if (angle > 185)
            angle -= 360;
        if (angle < -185)
            angle += 360;
    }

    void Turn()
    {
        if (barrelbody == null)
            return;
        if (angle > 0.1)
            TurnRight();
        if (angle < -0.1)
            TurnLeft();
    }

    void TurnLeft()
    {
        Debug.Log("left");
        if (Mathf.Abs(angle) - 100 * Time.deltaTime < 0)
        {
            barrelbody.transform.eulerAngles -= new Vector3(0f, Mathf.Abs(angle), 0f);
            angle = 0;
            return;
        }
        barrelbody.transform.eulerAngles -= new Vector3(0f, 100 * Time.deltaTime, 0f);
        angle += 100 * Time.deltaTime;

    }

    void TurnRight()
    {
        if(Mathf.Abs(angle) - 100 * Time.deltaTime < 0)
        {
            barrelbody.transform.eulerAngles += new Vector3(0f, Mathf.Abs(angle), 0f);
            angle = 0;
            return;
        }
        barrelbody.transform.eulerAngles += new Vector3(0f, 100 * Time.deltaTime, 0f);
        angle -= 100 * Time.deltaTime;
    }

    

    public void setTroopTarget(GameObject t)
    {
        if (!thisUnit.isTroopAttackable())
            return;
        if (t == null)
            return;
        InstructionQueue queue;
        int currentTroop = load.getCurrentTroops();
        if (currentTroop <= 0)
            return;
        //Debug.Log(currentTroop);
        for (int i = 0; i < currentTroop; i++)
        {
            queue = load.getTroops()[i].GetComponent<InstructionQueue>();
            if (!queue.isIdle())
                continue;
            queue.addInstruction(new Instruction(1, t));
            if(thisUnit.isTroopOut())
            {
                Debug.Log(queue.Count());
                queue.addInstruction(new Instruction(2, transform.parent.gameObject));
            }
        }
        if (thisUnit.isTroopOut())
            load.removeAllTroop();

    }

    public void ForceSetTroopTarget(GameObject t)
    {
        if (!thisUnit.isTroopAttackable())
            return;
        InstructionQueue queue;
        int currentTroops = load.getCurrentTroops();
        if (currentTroops <= 0)
            return;
        //Debug.Log(currentTroops);
        GameObject[] troops = load.getTroops();
        for (int i = 0; i < currentTroops; i++)
        {
            queue = troops[i].GetComponent<InstructionQueue>();
            //if (!queue.isIdle())
              //  continue;
            queue.clearAndExecute(new Instruction(1, t));
            if (thisUnit.isTroopOut())
            {
                queue.addInstruction(new Instruction(2, transform.parent.gameObject));
            }
        }
        if (thisUnit.isTroopOut())
            load.removeAllTroop();
    }

    public void TroopAttack()
    {
        if (!thisUnit.isTroopAttackable())
            return;
        Attack attack;
        GameObject[] troops = load.getTroops();
        int currentTroops = load.getCurrentTroops();
        if (currentTroops <= 0)
            return;
        for(int i = 0; i < currentTroops; i++)
        {
            if (!troops[i].transform.GetChild(3).TryGetComponent<Attack>(out attack))
                continue;
            attack.attack();
        }
    }

    public void setActive(bool active)
    {
        this.active = active;
    }

    bool barrelSpinning()
    {
        if (in_vehicle)
            return true;
        if (barrel.Equals(barrelbody) && barrel.Equals(transform.parent.gameObject))
            return true;
        if (locked && barrelbody != null)
        {
            //barrelbody.transform.LookAt(target.transform);
            barrelbody.transform.eulerAngles = Quaternion.LookRotation((target.transform.position - barrelbody.transform.position).normalized).eulerAngles + BarrelBodyFixData;
            barrelbody.transform.eulerAngles = new Vector3(0, barrelbody.transform.eulerAngles.y, 0);
            if (barrel != null)
            {
                barrel.transform.LookAt(target.transform);
                barrel.transform.eulerAngles = Quaternion.LookRotation((target.transform.position - barrel.transform.position).normalized).eulerAngles + BarrelFixData;
                if ((thisUnit.isFlyUnit() && barrel.transform.eulerAngles.x < 0) || (!thisUnit.isFlyUnit() && target.transform.position.y < barrel.transform.position.y))
                {   
                    barrel.transform.eulerAngles = new Vector3(0, barrel.transform.eulerAngles.y, barrel.transform.eulerAngles.z);
                }
                //barrel.transform.eulerAngles += BarrelFixData;
            }
        }
        else if (barrelbody != null && barrelbody.Equals(barrel))
        {
            Vector3 direction = (target.transform.position - barrelbody.transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            if (Mathf.Abs(barrelbody.transform.eulerAngles.x - rotation.eulerAngles.x) > 5 || Mathf.Abs(barrelbody.transform.eulerAngles.y - rotation.eulerAngles.y) > 5)
            {
                barrelbody.transform.rotation = Quaternion.Slerp(barrelbody.transform.rotation, rotation, 10 * Time.deltaTime);
                barrelbody.transform.eulerAngles += BarrelBodyFixData;
                return false;
            }
            else
                locked = true;
        }
        else
        {
            if (Mathf.Abs(angle) > 0.1)
            {
                Turn();
                return false;
            }
            else
                locked = true;
            if (barrel != null && barrel.transform.parent != null)
            {
                barrel.transform.LookAt(target.transform);
                barrel.transform.eulerAngles += BarrelFixData;
                if ((thisUnit.isFlyUnit() && barrel.transform.eulerAngles.x < 0) || (!thisUnit.isFlyUnit() && barrel.transform.eulerAngles.x > 0))
                    barrel.transform.eulerAngles = new Vector3(0, barrel.transform.eulerAngles.y, barrel.transform.eulerAngles.z);
                if (transform.parent.GetChild(0).name.Equals("Drone"))
                    barrel.transform.eulerAngles -= new Vector3(-90, 0, 0);
            }
        }
        return true;
    }
    void Chase()
    {
        if (in_vehicle)
            return;
        if (!sta && !in_range && control.isIdle())
        {
            queue.addAndStash(new Instruction(0, target.transform.position));
            attacking = true;
        }
        else if (in_range && !sta && queue.getCurrentType() != 1)
        {
            control.cancelTarget();
            queue.nextInstruction();
        }
    }
    public GameObject getBullet()
    {
        return bullet;
    }

    public GameObject[] getBulletOut()
    {
        return bulletOut;
    }

    public void setDisabeld(float time)
    {
        disabled = true;
        disabledTime = time;
        currentTime = Time.time;
    }

    public virtual void setTarget(GameObject gameobj)
    {
        if (gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() == thisUnit.getGroup())
            return;
        in_range = false;
        currentName = gameobj.name;
        /*if(!TargetUnits.Contains(gameobj.name))
        {
            TargetList.Add(gameobj);
            TargetUnits.Add(currentName);
        }*/
        target = gameobj;
        attacking = true;
        locked = false;
        sTarget = gameobj;
        set_initial = false;
        ForceSetTroopTarget(gameobj);
        calculate();
    }

    public GameObject getTarget()
    {
        return target;
    }

    /*public void cancelAttack()
    {
        target = null;
        TargetList = new List<GameObject>();
        attacking = false;
        if (animator != null)
            animator.SetBool("Attack", false);
    }*/

    public virtual void stopAttack()
    {
        target = null;
        sTarget = null;
        attacking = false;
        locked = false;
        in_range = false;
    }

    public void setBarrel(GameObject BarrelBody, GameObject Barrel, GameObject Bullet, GameObject[] bulletOut)
    {
        //stopAttack();
        if (BarrelBody != null)
        {
            barrelbody = BarrelBody;
            Debug.Log(barrelbody.transform.eulerAngles.x + " " + barrelbody.transform.eulerAngles.y + " " + barrelbody.transform.eulerAngles.z);
            BarrelBodyFixData = getFixData(BarrelBody.transform);
        }
        if(Barrel != null)
        {
            barrel = Barrel;
            BarrelFixData = getFixData(Barrel.transform);
        }
        if (Bullet != null)
            bullet = Bullet;
        if(bulletOut != null)
        {
            this.bulletOut = new GameObject[bulletOut.Length];
            bulletOut.CopyTo(this.bulletOut, 0);
        }
        SphereCollider collider;
        if (gameObject.TryGetComponent<SphereCollider>(out collider))
            radius = collider.radius;
        else
            radius = gameObject.GetComponent<CapsuleCollider>().radius;
    }

    public void resetBarrel()
    {
        stopAttack();
        barrelbody = initialBarrelBody;
        barrel = initialBarrel;
        bulletOut = new GameObject[initialBulletOut.Length];
        initialBulletOut.CopyTo(bulletOut, 0);
        radius = gameObject.GetComponent<SphereCollider>().radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 3 && other.gameObject.layer != 8)
            return;
        if (other.transform.childCount == 0)
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>() == null)
            return;
        if (thisUnit == null)
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() == thisUnit.getGroup())
            return;
        if (thisUnit.getAntiType()[other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getUnitType() / 10] == -1)
            return;
        if (TargetUnits.Contains(other.gameObject.name))
            return;
        TargetList.Add(other.gameObject);
        TargetUnits.Add(other.name);
    }

    public void attack()
    {
        TroopAttack();
        if(bursting)
        {
            if(currentBurst >= thisUnit.getBurst())
            {
                bursting = false;
                currentBurst = 0;
                time = Time.time;
                return;
            }
            if(Time.time - time >= 0.05f)
            {
                if (animator != null)
                    animator.SetTrigger("Attacking");
                if (index >= bulletOut.Length)
                    index = 0;
                for (int j = 0; j < thisUnit.getPellet(); j++)
                {
                    GameObject bGameobj = Instantiate(bullet, bulletOut[index].transform.position, Quaternion.identity);
                    bGameobj.GetComponent<Bullet>().Attack(target, bulletOut[index], thisUnit.getDamage(), thisUnit.getArmorPenetration(), thisUnit.getAccuracy(), thisUnit.getAntiType());
                }
                index++;
                currentBurst++;
                time = Time.time;
            }
        }
        else if (in_range && (sta || (control != null && transform.parent.GetComponent<MovementControl>().isIdle())))
        {
            ForceSetTroopTarget(target);
            if ((Time.time - time) >= thisUnit.getFireRate() && thisUnit.getDamage() > 0)
            {
                bursting = true;
                time = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (target == null)
          //  return;
        if(!sta && target != null && other.gameObject.Equals(target))
        {
            in_range = false;
            return;
        }
        if (other.gameObject.layer != 3 && other.gameObject.layer != 8)
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() == thisUnit.getGroup())
            return;
        if (target != null && target.Equals(other.gameObject) && !sta && !in_vehicle && target.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() > 0)
            return;
        TargetList.Remove(other.gameObject);
        TargetUnits.Remove(other.name);
        if (target == null || !target.Equals(other.gameObject))
            return;
        target = null;
        locked = false;
        queue.nextInstruction();
        if (transform.parent.GetChild(0).Equals("AntiAirCanon"))
        {
            Debug.Log(transform.parent.name);
            Debug.Break();
        }
    }

    public void setInRange()
    {
        in_range = true;
    }

    private void getDistance()
    {
        if (target == null)
        {
            distance = 0f;
            return;
        }
            
        distance = Mathf.Sqrt(Mathf.Pow(target.transform.position.x - thisUnitObject.transform.parent.position.x, 2)
            + Mathf.Pow(target.transform.position.z - thisUnitObject.transform.parent.position.z, 2));
    }

    public List<GameObject> getTargetList()
    {
        return TargetList.ToList();
    }

    public void AddTarget(GameObject unit)
    {
        if (unit.layer != 3 && unit.layer != 8)
            return;
        if (unit.transform.childCount == 0)
            return;
        if (unit.transform.GetChild(0).GetComponent<UnitLoad>() == null)
            return;
        if (thisUnit == null)
            return;
        if (unit.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() == thisUnit.getGroup())
            return;
        if (thisUnit.getAntiType()[unit.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getUnitType() / 10] == -1)
            return;
        if (TargetUnits.Contains(unit.name))
            return;
        if (unit.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() <= 0)
            return;
        TargetList.Add(unit);
        TargetUnits.Add(unit.name);
    }

    public void RemoveTarget(GameObject unit)
    {
        if(unit == null)
        {
            TargetList.Remove(null);
            return;
        }
        if (!sta && target != null && unit.Equals(target) && target.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() > 0)
        {
            in_range = false;
            return;
        }
        if (unit.layer != 3 && unit.layer != 8)
            return;
        if (unit.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() == thisUnit.getGroup())
            return;
        if (target != null && target.Equals(unit) && !sta && !in_vehicle && target.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() > 0)
            return;
        TargetList.Remove(unit);
        TargetUnits.Remove(unit.name);
        if (target != null && target.Equals(unit.gameObject))
        {
            target = null;
            locked = false;
        }
    }

    public void setRadius(float radius)
    {
        this.radius = radius;
    }

    public bool InTargetList(string name)
    {
        return TargetUnits.Contains(name);
    }

    public void SetBarrelFixData(Vector3 fix)
    {
        BarrelFixData = fix;
    }

    Vector3 getFixData(Transform obj)
    {
        Transform tmp = obj;


        while(obj.parent != null)
            obj = obj.parent;

        return tmp.eulerAngles - obj.eulerAngles;
    }

    public void SetBarrelBodyFixData(Vector3 fix)
    {
        BarrelBodyFixData = fix;
    }


    public GameObject NextTarget()
    {
        TargetList.Remove(null);
        List<GameObject> list = TargetList.ToList();
        GameObject result = null;
        float dist = int.MaxValue;
        float current;
        bool hasNull = false;

        for (int i = 0; i < list.Count; i++)
        {
            if(list[i] == null)
            {
                hasNull = true;
                continue;
            }
            current = (transform.position.x - list[i].transform.position.x) * (transform.position.x - list[i].transform.position.x) +
            (transform.position.z - list[i].transform.position.z) * (transform.position.z - list[i].transform.position.z);
            if (current < dist)
            {
                result = list[i];
                dist = current;
            }
        }

        if (hasNull)
        {
            TargetList.Clear();
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    TargetList.Add(list[i]);
            }
        }

        if (result == null)
            TargetList.Clear();

        return result;
    }
}
